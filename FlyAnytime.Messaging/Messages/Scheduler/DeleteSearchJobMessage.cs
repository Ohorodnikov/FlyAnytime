using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.Scheduler
{
    public class DeleteSearchJobMessage : BaseSearchJobMessage
    {
        public DeleteSearchJobMessage(long chatId, Guid settingsId) : base(chatId, settingsId) { }
    }
}
