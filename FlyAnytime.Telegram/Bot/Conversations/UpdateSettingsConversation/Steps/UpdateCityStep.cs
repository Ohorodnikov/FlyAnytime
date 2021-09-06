using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps
{
    public class UpdateCityStep : BaseConversationStep
    {
        public override Guid StepId => new Guid("057F1851-9210-4344-A427-10402D6D0313");

        public override bool WaitAnswer => true;

        public override async Task OnGetUserAnswer(IBotHelper bot, long chatId, object response)
        {
            throw new NotImplementedException();
        }

        public override async Task<Message> SendConversationBotMessage(IBotHelper bot, long chatId)
        {
            throw new NotImplementedException();
        }
    }
}
