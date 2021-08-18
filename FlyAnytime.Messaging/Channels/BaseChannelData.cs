using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Channels
{
    public interface IChannelData
    {
        Guid MessageId { get; }
        void GenerateMessageId();
    }

    public class BaseChannelData : IChannelData
    {
        public Guid MessageId { get; private set; }

        void IChannelData.GenerateMessageId()
        {
            MessageId = Guid.NewGuid();
        }
    }
}
