using FlyAnytime.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
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

        public RabbitMessageBus(IRabbitConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _consumerChannel = CreateChannel();
        }

        private IModel CreateChannel()
        {
            _connection.TryConnect();

            return _connection.CreateModel();
        }

        private string GetChannelKey<TMessage>()
        {
            return typeof(TMessage).Name;
        }

        private byte[] Obj2Bytes<TObj>(TObj obj) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

        private TMessage GetMessage<TMessage>(BasicDeliverEventArgs ea) => JsonConvert.DeserializeObject<TMessage>(Encoding.UTF8.GetString(ea.Body.ToArray()));

        private void QueueDeclare(string chatName)
        {
            _consumerChannel.QueueDeclare(chatName, true, false, false, null);
        }

        private void DoPublish<TMessage>(string chatName, TMessage msg, IBasicProperties props = null)
            where TMessage : BaseMessage
        {
            _consumerChannel.BasicPublish("", chatName, mandatory: true, props, Obj2Bytes(msg));
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
            var chatName = GetChannelKey<TMessage>();

            QueueDeclare(chatName);
            _consumerChannel.BasicQos(0, 1, false);
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += async (sender, ea) => await OnMessageReceive(sender, ea, onReceive);

            _consumerChannel.BasicConsume(chatName, autoAck: false, consumer: consumer);
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
            var chatName = GetChannelKey<TMessageSend>();
            QueueDeclare(chatName);
            DoPublish(chatName, message);
        }

        public async Task<TMessageResult> Publish<TMessageSend, TMessageResult>(TMessageSend message)
            where TMessageSend : BaseMessage
            where TMessageResult : BaseResponseMessage<TMessageSend>
        {
            var chat = GetChannelKey<TMessageSend>();
            QueueDeclare(chat);

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
