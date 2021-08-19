using FlyAnytime.Messaging.Channels;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Messaging
{
    public class Subscribe
    {
        public static void SubscribeWithoutResult<TChannelData>(IChannelDescriptor<TChannelData> channelDescriptor, Action<TChannelData> onMessageRecieve)
            where TChannelData : IChannelData
        {
            var channel = Connection.CreateChannel();
            var chat = channelDescriptor.ChannelName;
            channel.QueueDeclare(chat, true, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var obj = channelDescriptor.Message2Data(message);
                    onMessageRecieve(obj);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(chat, autoAck: false, consumerTag: string.Join(",", consumer.ConsumerTags), noLocal: false, exclusive: false, arguments: null, consumer: consumer);
        }

        public static void SubscribeReturnResult<TChannelData, TResultData>(IChannelWithResultDescriptor<TChannelData, TResultData> channelDescriptor, Func<TChannelData, TResultData> onMessageRecieve)
            where TChannelData : IChannelData
            where TResultData : IChannelData
        {
            var channel = Connection.CreateChannel();

            var chat = channelDescriptor.ChannelName;

            channel.QueueDeclare(chat, true, false, false, null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var response = default(TResultData);

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var obj = channelDescriptor.Message2Data(message);
                    response = onMessageRecieve(obj);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    var respAsStr = channelDescriptor.ResultData2String(response);
                    var responseBytes = Encoding.UTF8.GetBytes(respAsStr);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes, mandatory: true);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(chat, autoAck: false, consumerTag: string.Join(",", consumer.ConsumerTags), noLocal: false, exclusive: false, arguments: null, consumer: consumer);
        }
    }
}
