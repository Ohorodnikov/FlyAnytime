using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation
{
    public class UpdateSettingsCountryCityConversation : ConversationBase
    {
        static readonly Guid _id = new Guid("64A93223-5914-46C9-AF7C-BF6DA7FC6919");

        public UpdateSettingsCountryCityConversation(IBotHelper bot) : base(bot, _id) { }

        public override void InitConversationSteps(IConversationStepIniter stepIniter)
        {
            stepIniter
                .SetNextStep(new UpdateCountryStep())
                .SetNextStep(new UpdateCityStep())
                ;
        }
    }
}
