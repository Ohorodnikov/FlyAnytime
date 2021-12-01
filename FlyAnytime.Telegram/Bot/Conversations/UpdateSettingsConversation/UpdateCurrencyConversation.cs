using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation
{
    public class UpdateCurrencyConversation : ConversationBase
    {
        static readonly Guid _id = new Guid("D3448782-51CA-422A-991A-201037FD3B4B");
        public UpdateCurrencyConversation(IBotHelper bot) : base(bot, _id)
        {
        }

        public override void InitConversationSteps(IConversationStepIniter stepIniter)
        {
            stepIniter
                .SetNextStep(new ChangeCurrencyStep());
        }
    }
}
