using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class AddOrUpdateCountryHandler : IMessageHandler<AddOrUpdateCountryMessage>
    {
        private readonly TelegramContext _dbContext;
        private readonly ILocalizationHelper _localizationHelper;

        public AddOrUpdateCountryHandler(TelegramContext dbContext, ILocalizationHelper localizationHelper)
        {
            _dbContext = dbContext;
            _localizationHelper = localizationHelper;
        }

        public async Task Handle(AddOrUpdateCountryMessage message)
        {
            var savedCountry = _dbContext.Set<Country>().FirstOrDefault(x => x.Code == message.Code);

            if (savedCountry == null)
                await AddContry(message);
            else
                await UpdateCountry(savedCountry, message);
        }

        private async Task AddContry(AddOrUpdateCountryMessage message)
        {
            var country = new Country
            {
                Code = message.Code,
                Name = message.Name,
                CurrencyCode = message.CurrencyToSearch
            };

            _dbContext.Add(country);
            await _dbContext.SaveChangesAsync();

            await _localizationHelper.AddOrUpdateEntityLocalizations(country, message.LanguageCode2Value);
        }

        private async Task UpdateCountry(Country country, AddOrUpdateCountryMessage message)
        {
            country.Name = message.Name;
            country.CurrencyCode = message.CurrencyToSearch;

            await _dbContext.SaveChangesAsync();

            await _localizationHelper.AddOrUpdateEntityLocalizations(country, message.LanguageCode2Value);
        }
    }
}
