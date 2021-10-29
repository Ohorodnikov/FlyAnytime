using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.Scheduler
{
    public abstract class BaseSearchJobMessage : BaseMessage
    {
        protected BaseSearchJobMessage(long chatId, Guid settingsId)
        {
            ChatId = chatId;
            SettingsId = settingsId;
        }

        public long ChatId { get; private set; }
        public Guid SettingsId { get; private set; }
    }
}
