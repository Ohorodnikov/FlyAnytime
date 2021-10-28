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
    public class AddOrUpdateCityHandler : IMessageHandler<AddOrUpdateCityMessage>
    {
        private readonly TelegramContext _dbContext;
        private readonly ILocalizationHelper _localizationHelper;

        public AddOrUpdateCityHandler(TelegramContext dbContext, ILocalizationHelper localizationHelper)
        {
            _dbContext = dbContext;
            _localizationHelper = localizationHelper;
        }

        public async Task Handle(AddOrUpdateCityMessage message)
        {
            var country = _dbContext.Set<Country>().FirstOrDefault(x => x.Code == message.CountryCode);

            if (country == null)
                return;

            var savedCity = _dbContext.Set<City>().FirstOrDefault(x => x.Code == message.Code);

            if (savedCity == null)
                await AddCity(message, country);
            else
                await UpdateCity(savedCity, message, country);
        }

        private async Task AddCity(AddOrUpdateCityMessage message, Country country)
        {
            var city = new City
            {
                Code = message.Code,
                Name = message.Name,
                Country = country
            };

            _dbContext.Add(city);
            await _dbContext.SaveChangesAsync();

            await _localizationHelper.AddOrUpdateEntityLocalizations(city, message.LanguageCode2Value);
        }

        private async Task UpdateCity(City city, AddOrUpdateCityMessage message, Country country)
        {
            city.Country = country;
            city.Name = message.Name;

            await _dbContext.SaveChangesAsync();

            await _localizationHelper.AddOrUpdateEntityLocalizations(city, message.LanguageCode2Value);
        }
    }
}
