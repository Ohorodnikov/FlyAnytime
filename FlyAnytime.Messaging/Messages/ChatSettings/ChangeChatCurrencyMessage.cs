using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.ChatSettings
{
    public class ChangeChatCurrencyMessage : BaseMessage
    {
        public ChangeChatCurrencyMessage(string currencyCode, long chatId)
        {
            CurrencyCode = currencyCode;
            ChatId = chatId;
        }

        public long ChatId { get; }
        public string CurrencyCode { get; }
    }
}
