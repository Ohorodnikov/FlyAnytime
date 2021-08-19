using FlyAnytime.Messaging.Channels;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Messaging
{
    public static class Publish
    {
        public static void FireAndForget<TChannelData>(IChannelDescriptor<TChannelData> channelDescriptor)
            where TChannelData : IChannelData
        {
            channelDescriptor.Data.GenerateMessageId();

            var channel = Connection.CreateChannel();
            channel.QueueDeclare(channelDescriptor.ChannelName, true, false, false, null);

            var body = Encoding.UTF8.GetBytes(channelDescriptor.Data2String());

            channel.BasicPublish("", channelDescriptor.ChannelName, true, null, body);
        }

        public static async Task<TResultData> FireAndGetResult<TChannelData, TResultData>(IChannelWithResultDescriptor<TChannelData, TResultData> channelDescriptor)
            where TChannelData : IChannelData
            where TResultData : IChannelData
        {
            channelDescriptor.Data.GenerateMessageId();
            var chat = channelDescriptor.ChannelName;

            var callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<TResultData>>();

            var channel = Connection.CreateChannel();
            var replyQueueName = channel.QueueDeclare("", true, false, false, null).QueueName;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<TResultData> tcs))
                    return;
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);

                tcs.TrySetResult(channelDescriptor.ResultString2Data(response));
            };

            channel.BasicConsume(
              consumer: consumer,
              queue: replyQueueName,
              autoAck: true,
              consumerTag: string.Join(",", consumer.ConsumerTags),
              noLocal: false,
              exclusive: false,
              arguments: null);


            var props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;
            var messageBytes = Encoding.UTF8.GetBytes(channelDescriptor.Data2String());
            var tcs = new TaskCompletionSource<TResultData>();
            callbackMapper.TryAdd(correlationId, tcs);

            channel.BasicPublish(
                exchange: "",
                routingKey: chat,
                basicProperties: props,
                body: messageBytes,
                mandatory: true);

            //cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out var tmp));
            return await tcs.Task;
        }
    }
}
