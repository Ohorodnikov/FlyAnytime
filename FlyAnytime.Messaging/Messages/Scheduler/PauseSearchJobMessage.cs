using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.Scheduler
{
    public class PauseSearchJobMessage : BaseSearchJobMessage
    {
        protected PauseSearchJobMessage(long chatId, Guid settingsId) : base(chatId, settingsId)
        {
        }
    }
}
