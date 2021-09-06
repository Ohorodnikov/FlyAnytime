using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.InlineKeyboardButtons
{
    public class UpdateCountryAndCitySettingsButton : InlineKeyboardButtonWithActionBase
    {
        public UpdateCountryAndCitySettingsButton() : base("Set country and city", "2EEA44EC-D732-4489-B462-B586DCAE9863")
        {
        }

        public override async Task OnButtonPress(IBotHelper bot, Message message)
        {
            await new UpdateSettingsCountryCityConversation(bot).Start(message.Chat.Id);
        }
    }
}
