using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.ChatSettings
{
    public class UpdateChatLanguageMessage : BaseMessage
    {
        public UpdateChatLanguageMessage(long chatId, string languageCode)
        {
            ChatId = chatId;
            LanguageCode = languageCode;
        }

        public long ChatId { get; private set; }
        public string LanguageCode { get; private set; }
    }
}
