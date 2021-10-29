using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.SearchEngine.Exceptions;
using FlyAnytime.Tools;
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.Engine
{
    public interface ISearchEngine
    {
        Task<IEnumerable<OneResult>> Search(MakeSearchMessage settings); 
    }

    public class SearchEngine : ISearchEngine
    {
        private void ValidateSettings(MakeSearchMessage settings)
        {
            if (settings.CityFlyFrom.IsNullOrEmpty())
                throw new DataValidationException($"City to fly from is empty for chat {settings.ChatId}");

            if (settings.AirportsFlyTo == null || !settings.AirportsFlyTo.Any())
                throw new DataValidationException($"Airports to fly to is empty for chat {settings.ChatId}");

            if (settings.PriceSettings == null)
                throw new DataValidationException($"No Price settings for chat {settings.ChatId}");

            if (settings.SearchFrame == null)
                throw new DataValidationException($"No search frame settings for chat {settings.ChatId}");

            if (settings.TripDuration == null)
                throw new DataValidationException($"No trip duration settings for chat {settings.ChatId}");

            if (settings.TripDuration.DaysMax < settings.TripDuration.DaysMin)
                throw new DataValidationException($"Incorrect Trip duration for chat {settings.ChatId}: Days max less than Days min");

            if (settings.PriceSettings.Amount < 0)
                throw new DataValidationException($"Incorrect Price settings for chat {settings.ChatId}: Amount must me greater than 0");

            if (settings.PriceSettings.Type == SearchPriceSettingsType.PercentDiscount && settings.PriceSettings.Amount > 100)
                throw new DataValidationException($"Incorrect Price settings for chat {settings.ChatId}: Discount must be between 0% and 100%. Actual value is {settings.PriceSettings.Amount}");

            if (settings.SearchFrame.End < settings.SearchFrame.Start)
                throw new DataValidationException($"Incorrect Search frame for chat {settings.ChatId}: Back flight must me later than first flight");

        }

        public async Task<IEnumerable<OneResult>> Search(MakeSearchMessage settings)
        {
            ValidateSettings(settings);

            var res = new List<OneResult>();

            var oneRes1 = new OneResult
            {
                CityFrom = settings.CityFlyFrom,
                CityTo = "NY",
                DateTimeFrom = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(35)),
                DateTimeBack = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(40)),
                Price = 1488,
                DiscountPercent = 10,
                ResultUrl = ""
            };

            var oneRes2 = new OneResult
            {
                CityFrom = settings.CityFlyFrom,
                CityTo = "NY",
                DateTimeFrom = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(35)),
                DateTimeBack = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(40)),
                Price = 1488,
                DiscountPercent = 20,
                ResultUrl = ""
            };

            var oneRes3 = new OneResult
            {
                CityFrom = settings.CityFlyFrom,
                CityTo = "NY",
                DateTimeFrom = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(35)),
                DateTimeBack = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(40)),
                Price = 1488,
                DiscountPercent = 30,
                ResultUrl = ""
            };

            res.Add(oneRes1);
            res.Add(oneRes2);
            res.Add(oneRes3);

            return res;
        }
    }
}
