using FlyAnytime.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyAnytime.Messaging
{
    public interface IMessageBus
    {
        void Publish<TMessageSend>(TMessageSend message) 
            where TMessageSend : BaseMessage;

        Task<TMessageResult> Publish<TMessageSend, TMessageResult>(TMessageSend message)
            where TMessageSend : BaseMessage
            where TMessageResult : BaseResponseMessage<TMessageSend>;

        void Subscribe<TMessage, THandler>()
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>;
        void Subscribe<TMessage, THandler, TMessageResult>()
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage, TMessageResult>
            where TMessageResult : BaseResponseMessage<TMessage>;
    }

    public class RabbitMessageBus : IMessageBus
    {
        private IModel _consumerChannel;
        private readonly IRabbitConnection _connection;
        private readonly IServiceProvider _serviceProvider;

        ILogger logger;

        public RabbitMessageBus(IRabbitConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            logger = _serviceProvider.GetService<ILogger<RabbitMessageBus>>();
            _consumerChannel = CreateChannel();
        }

        private IModel CreateChannel()
        {
            _connection.TryConnect();

            return _connection.CreateModel();
        }

        private string GetChannelKey(Type msgType)
        {
            return msgType.Name;
        }

        private byte[] Obj2Bytes<TObj>(TObj obj) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

        private TMessage GetMessage<TMessage>(BasicDeliverEventArgs ea) => JsonConvert.DeserializeObject<TMessage>(Encoding.UTF8.GetString(ea.Body.ToArray()));

        //private void QueueDeclare(string chatName)
        //{
        //    var q = _consumerChannel.QueueDeclare(queue: chatName,
        //                                          durable: false,
        //                                          exclusive: false,
        //                                          autoDelete: false,
        //                                          arguments: null);
        //}

        private void ExchangeDeclare(string chatName)
        {
            _consumerChannel.ExchangeDeclare(chatName, ExchangeType.Fanout);
        }

        private void DoPublish<TMessage>(string chatName, TMessage msg, IBasicProperties props = null)
            where TMessage : BaseMessage
        {
            logger.LogInformation($"{DateTime.Now}: Send message {msg.GetType()} for exchange {chatName}");

            _consumerChannel.BasicPublish(exchange: chatName,
                                          routingKey: "",
                                          //mandatory: true,
                                          basicProperties: props,
                                          body: Obj2Bytes(msg));
        }

        #region Subscribe

        public void Subscribe<TMessage, THandler>()
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>
        {
            DoSubscribe<TMessage, THandler>(async (dea, msg, handlers) => await SubscribeWOResult(dea, msg, handlers));
        }

        public void Subscribe<TMessage, THandler, TMessageResult>()
            where TMessage : BaseMessage
            where TMessageResult : BaseResponseMessage<TMessage>
            where THandler : IMessageHandler<TMessage, TMessageResult>
        {
            DoSubscribe<TMessage, THandler>(async (dea, msg, handlers) => await SubscribeReturnResult<TMessage, THandler, TMessageResult>(dea, msg, handlers));
        }

        private void DoSubscribe<TMessage, THandler>(Func<BasicDeliverEventArgs, TMessage, IEnumerable<THandler>, Task> onReceive)
            where TMessage : BaseMessage
            where THandler : IMessageHandler
        {
            var chatName = GetChannelKey(typeof(TMessage));

            ExchangeDeclare(chatName);
            var q = _consumerChannel.QueueDeclare().QueueName;
            _consumerChannel.QueueBind(q, chatName, "");

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += async (sender, ea) => await OnMessageReceive(sender, ea, onReceive);

            _consumerChannel.BasicConsume(consumer: consumer,
                                          queue: q,
                                          autoAck: false,
                                          consumerTag: "",
                                          noLocal: false,
                                          exclusive: false,
                                          arguments: null);
        }

        private async Task SubscribeWOResult<TMessage, THandler>(BasicDeliverEventArgs ea, TMessage message, IEnumerable<THandler> handlers)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>
        {
            foreach (var handler in handlers)
            {
                await handler.Handle(message);
            }
        }

        private async Task SubscribeReturnResult<TMessage, THandler, TMessageResult>(BasicDeliverEventArgs ea, TMessage message, IEnumerable<THandler> handlers)
            where TMessage : BaseMessage
            where TMessageResult : BaseResponseMessage<TMessage>
            where THandler : IMessageHandler<TMessage, TMessageResult>
        {
            var props = ea.BasicProperties;
            var replyProps = _consumerChannel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;


            if (handlers.Count() > 1)
            {
                throw new InvalidOperationException("Supported only 1 subscriber on event with result message");
            }

            TMessageResult result = null;
            if (handlers.Count() ==  1)
            {
                var handler = handlers.ElementAt(0);
                result = await handler.Handle(message);
                result.Request = message;
            }

            _consumerChannel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: Obj2Bytes(result), mandatory: true);
        }

        private async Task OnMessageReceive<TMessage, THandler>(object sender, BasicDeliverEventArgs ea, Func<BasicDeliverEventArgs, TMessage, IEnumerable<THandler>, Task> act)
            where TMessage : BaseMessage
            where THandler : IMessageHandler
        {
            using var scope = _serviceProvider.CreateScope();
            var allHandlers = scope.ServiceProvider.GetServices<THandler>();
            var log = scope.ServiceProvider.GetService<ILogger<RabbitMessageBus>>();
            log.LogInformation($"{DateTime.Now}: Receive {typeof(TMessage)} on {typeof(THandler)}");
            try
            {
                await act(ea, GetMessage<TMessage>(ea), allHandlers);
            }
            catch (Exception)
            {

            }
            finally
            {
                _consumerChannel.BasicAck(ea.DeliveryTag, false);
            }
        }

        #endregion

        #region Publish

        public void Publish<TMessageSend>(TMessageSend message) where TMessageSend : BaseMessage
        {
            var chatName = GetChannelKey(message.GetType());
            ExchangeDeclare(chatName);
            //QueueDeclare(chatName);
            var props = _consumerChannel.CreateBasicProperties();

            props.Persistent = true;

            DoPublish(chatName, message, props);
        }

        public async Task<TMessageResult> Publish<TMessageSend, TMessageResult>(TMessageSend message)
            where TMessageSend : BaseMessage
            where TMessageResult : BaseResponseMessage<TMessageSend>
        {
            var chat = GetChannelKey(message.GetType());
            //QueueDeclare(chat);
            ExchangeDeclare(chat);
            var replyQueueName = _consumerChannel.QueueDeclare("", true, false, false, null).QueueName;

            var tcs = new TaskCompletionSource<TMessageResult>();

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            consumer.Received += async (model, ea) =>
            {
                var msg = GetMessage<TMessageResult>(ea);
                msg.IsResponseFromSubscriber = true;

                tcs.TrySetResult(msg);
            };

            _consumerChannel.BasicConsume(replyQueueName, true, consumer);

            var props = _consumerChannel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            DoPublish(chat, message, props);

            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            ct.Token.Register(() => tcs.TrySetCanceled());

            try
            {
                return await tcs.Task;
            }
            catch (TaskCanceledException e)
            {
                var resultBase = (TMessageResult)Activator.CreateInstance(typeof(TMessageResult), true);
                resultBase.IsResponseFromSubscriber = false;
                resultBase.Request = message;
                resultBase.ErrorMessage = "Wait for result was too long";
                return resultBase;
            }
            catch (Exception e)
            {
                var resultBase = (TMessageResult)Activator.CreateInstance(typeof(TMessageResult), true);
                resultBase.IsResponseFromSubscriber = false;
                resultBase.Request = message;
                resultBase.ErrorMessage = e.Message;
                return resultBase;
            }
        }

        #endregion
    }
}
