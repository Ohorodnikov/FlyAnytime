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
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class DeleteCountryHandler : IMessageHandler<DeleteCountryMessage>
    {
        private readonly TelegramContext _dbContext;
        private readonly ILocalizationHelper _localizationHelper;
        private readonly IBotHelper _botHelper;

        public DeleteCountryHandler(ILocalizationHelper localizationHelper, IBotHelper botHelper)
        {
            _dbContext = botHelper.DbContext;
            _localizationHelper = localizationHelper;
            _botHelper = botHelper;
        }

        public async Task Handle(DeleteCountryMessage message)
        {
            var country = _dbContext.Set<SearchCountry>().FirstOrDefault(x => x.Code == message.Code);
            if (country == null)
                return;

            var cities = _dbContext.Set<SearchCity>().Where(x => x.Country.Id == country.Id);

            foreach (var city in cities)
                _dbContext.RemoveRange(await _localizationHelper.GetEntityLocalizations(city));

            var countryLocs = await _localizationHelper.GetEntityLocalizations(country);
            _dbContext.RemoveRange(countryLocs);

            var chatsWithCurrentCountry = _dbContext.Set<Chat>()
                .Where(x => x.SearchSettings.ChatCountry.Id == country.Id)
                .ToList();

            foreach (var settings in chatsWithCurrentCountry.Select(x => x.SearchSettings))
            {
                settings.ChatCity = null;
                settings.ChatCountry = null;
            }

            _dbContext.RemoveRange(cities);
            _dbContext.Remove(country);

            await _dbContext.SaveChangesAsync();

            foreach (var chat in chatsWithCurrentCountry)
            {
                var countryLocal = countryLocs
                    .FirstOrDefault(x => x.Language.Id == chat.UserLanguage.Id)?.Localization
                    ?? country.Name
                    ;

                var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
                    {
                        new UpdateCountryAndCitySettingsButton(),
                    });

                await _botHelper.Bot.SendTextMessageAsync(
                    chat.Id, 
                    $"Your country {countryLocal} has been removed. Press button to set up new settings for country and city",
                    replyMarkup: inlineKeyboard
                    );
            }
        }
    }
}
