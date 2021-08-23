using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages
{
    [Serializable]
    public class NoPrivateCtorException : Exception
    {
        public NoPrivateCtorException() { }
        public NoPrivateCtorException(string message) : base(message) { }
        public NoPrivateCtorException(string message, Exception inner) : base(message, inner) { }
        public NoPrivateCtorException(Type t) : base($"Type {t.FullName} doesnt have private ctor without params") { }
        protected NoPrivateCtorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    public abstract class BaseMessage
    {
        public Guid MessageId { get; } = Guid.NewGuid();
        public DateTime CreationDateTime { get; } = DateTime.UtcNow;
    }

    public abstract class BaseResponseMessage<TRequestMessage> : BaseMessage
        where TRequestMessage : BaseMessage
    {
        public TRequestMessage Request { get; set; }
        public bool IsResponseFromSubscriber { get; set; }
        public string ErrorMessage { get; set; }
    }
}
