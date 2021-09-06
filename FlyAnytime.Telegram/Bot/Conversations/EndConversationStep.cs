using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public class EndConversationStep : BaseConversationStep
    {
        public override Guid StepId => new Guid("DFC39849-9845-4D4F-9B90-923324BBA1F2");

        public override bool WaitAnswer => false;

        public override async Task OnGetUserAnswer(IBotHelper bot, long chatId, object response)
        {
            throw new NotSupportedException();
        }

        public override async Task<Message> SendConversationBotMessage(IBotHelper bot, long chatId)
        {
            return null;
        }
    }
}
