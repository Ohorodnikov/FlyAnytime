using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps;
using System;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation
{
    public class UpdatePriceDectinationConversation : ConversationBase
    {
        static readonly Guid _id = new Guid("64A93223-5927-46C9-AF7C-BF6DA7FC6919");

        public UpdatePriceDectinationConversation(IBotHelper bot) : base(bot, _id) { }

        public override void InitConversationSteps(IConversationStepIniter stepIniter)
        {
            stepIniter
                .SetNextStep(new UpdatePriceAndCountryStep())
                ;
        }
    }
}
