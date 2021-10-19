using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.ChatSettings
{
    public class UpdateChatCityMessage : BaseMessage
    {
        public UpdateChatCityMessage(long chatId, string cityCode)
        {
            ChatId = chatId;
            CityCode = cityCode;
        }

        public long ChatId { get; private set; }
        public string CityCode { get; private set; }
    }
}
