using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Messaging.Messages
{
    public interface IMessageHandler
    {
    }

    public interface IMessageHandler<TMessage> : IMessageHandler
        where TMessage : BaseMessage
    {
        Task Handle(TMessage message);
    }

    public interface IMessageHandler<TMessageIn, TMessageOut> : IMessageHandler
        where TMessageIn : BaseMessage
        where TMessageOut : BaseResponseMessage<TMessageIn>
    {
        Task<TMessageOut> Handle(TMessageIn message);
    }
}
