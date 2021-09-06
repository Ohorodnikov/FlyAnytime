using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps
{
    public class UpdateCountryStep : BaseConversationStep
    {
        public override Guid StepId => new Guid("E87E072F-076A-45A7-B311-190534D4D791");

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
