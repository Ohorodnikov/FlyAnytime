using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Messaging
{
    public class Subscribe
    {
        public static void SubscribeWithoutResult(string chat, Action<string> onMessageRecieve)
        {
            var channel = Connection.CreateChannel();

            //channel.QueueDelete(chat, false, false);

            channel.QueueDeclare(chat, true, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    onMessageRecieve(message);
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

        public static void SubscribeReturnResult(string chat, Func<string, string> onMessageRecieve)
        {
            var channel = Connection.CreateChannel();

            channel.QueueDeclare(chat, true, false, false, null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                string response = null;

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    response = onMessageRecieve(message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes, mandatory: true);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(chat, autoAck: false, consumerTag: string.Join(",", consumer.ConsumerTags), noLocal: false, exclusive: false, arguments: null, consumer: consumer);
        }
    }
}
