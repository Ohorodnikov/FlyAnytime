using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.Telegram.Bot;
using FlyAnytime.Telegram.Bot.InlineKeyboardButtons;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class DeleteCityHandler : IMessageHandler<DeleteCityMessage>
    {
        private readonly TelegramContext _dbContext;
        private readonly ILocalizationHelper _localizationHelper;
        private readonly IBotHelper _botHelper;

        public DeleteCityHandler(ILocalizationHelper localizationHelper, IBotHelper botHelper)
        {
            _dbContext = botHelper.DbContext;
            _localizationHelper = localizationHelper;
            _botHelper = botHelper;
        }

        public async Task Handle(DeleteCityMessage message)
        {
            var city = _dbContext.Set<City>().FirstOrDefault(x => x.Code == message.Code);
            if (city == null)
                return;

            var cityLocs = await _localizationHelper.GetEntityLocalizations(city);
            _dbContext.RemoveRange(cityLocs);

            var chatsWithCurrentCity = _dbContext.Set<Chat>()
                .Where(x => x.ChatCity.Id == city.Id)
                .ToList();

            foreach (var chat in chatsWithCurrentCity)
            {
                chat.ChatCity = null;
                chat.ChatCountry = null;
            }

            _dbContext.Remove(city);

            foreach (var chat in chatsWithCurrentCity)
            {
                var cityLocal = cityLocs
                    .FirstOrDefault(x => x.Language.Id == chat.UserLanguage.Id)?.Localization
                    ?? city.Name
                    ;

                var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
                    {
                        new UpdateCountryAndCitySettingsButton(),
                    });

                await _botHelper.Bot.SendTextMessageAsync(
                    chat.Id,
                    $"Your city {cityLocal} has been removed. Press button to set up new settings for country and city",
                    replyMarkup: inlineKeyboard
                    );
            }
        }
    }
}
