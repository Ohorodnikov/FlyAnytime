using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps;
using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public class UpdateSettingsFullConversation : ConversationBase
    {
        static readonly Guid _id = new Guid("37F7C2D2-879D-4E3B-9136-E97DA878FFE0");
        public UpdateSettingsFullConversation(IBotHelper bot) : base(bot, _id) { }

        public override void InitConversationSteps(IConversationStepIniter stepIniter)
        {
            stepIniter
                .SetNextStep(new UpdateUserLanguageStep())
                .SetNextStep(new UpdateCountryStep())
                .SetNextStep(new UpdateCityStep())
                ;
        }
    }
}
