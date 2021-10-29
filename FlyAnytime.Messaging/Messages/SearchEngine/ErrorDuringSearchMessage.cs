using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchEngine
{
    public class ErrorDuringSearchMessage : BaseMessage
    {
        public ErrorDuringSearchMessage(string message, long chatId)
        {
            Message = message;
            ChatId = chatId;
        }

        public string Message { get; private set; }
        public long ChatId { get; private set; }
    }
}
