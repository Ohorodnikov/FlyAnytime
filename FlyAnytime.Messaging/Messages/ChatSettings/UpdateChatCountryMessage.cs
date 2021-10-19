using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.ChatSettings
{
    public class UpdateChatCountryMessage : BaseMessage
    {
        public UpdateChatCountryMessage(long chatId, string countryCode)
        {
            ChatId = chatId;
            CountryCode = countryCode;
        }

        public long ChatId { get; private set; }
        public string CountryCode { get; private set; }
    }
}
