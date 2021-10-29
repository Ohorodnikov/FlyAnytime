using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using FlyAnytime.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace FlyAnytime.Telegram.MessageHandlers
{
    class OneFlyResult
    {
        public DateTime FlyToStart { get; set; }
        public DateTime FlyBackEnd { get; set; }
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
        public string Culture { get; set; }

        private string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("ddd, dd MMM, HH:mm", new CultureInfo(Culture));
        }

        public override string ToString()
        {
            return $"- {FormatDateTime(FlyToStart)} - {FormatDateTime(FlyBackEnd)}, {Amount} {CurrencySymbol}";
        }
    }

    class OneCityResult
    {
        public string CityLocalName { get; set; }
        public string CountryLocalName { get; set; }
        public List<OneFlyResult> FlyResults { get; set; } = new List<OneFlyResult>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"#{CityLocalName}, {CountryLocalName}");

            foreach (var flight in FlyResults)
            {
                sb.AppendLine(flight.ToString());
            }

            return sb.ToString();
        }
    }


    public class GetResultsMessageHandler : IMessageHandler<SearchResultMessage>
    {
        private readonly ITelegramBotClient _tgClient;
        private readonly TelegramContext _dbContext;
        ILocalizationHelper _localizationHelper;
        public GetResultsMessageHandler(ITelegramBotClient tgClient, TelegramContext dbContext, ILocalizationHelper localizationHelper)
        {
            _dbContext = dbContext;
            _tgClient = tgClient;
            _localizationHelper = localizationHelper;
        }

        public async Task Handle(SearchResultMessage message)
        {
            var msg = FormatResult(message);

            await _tgClient.SendTextMessageAsync(message.ChatId, msg, ParseMode.Markdown);
        }


        /// <summary>
        /// #City, Country
        /// - dayOfWeek, dayOfMonth Month Time - dayOfWeek, dayOfMonth Month Time, Amount Currency
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private string FormatResult(SearchResultMessage message)
        {
            var dict = new Dictionary<string, OneCityResult>();
            foreach (var res in message.Results)
            {
                var ocr = dict.GetOrAdd(res.CityTo, cityCode => CreateResult(cityCode, message.ChatId).GetAwaiter().GetResult());

                var ofr = new OneFlyResult
                {
                    FlyToStart = DateTimeHelper.UnixToUtc(res.DateTimeFrom),
                    FlyBackEnd = DateTimeHelper.UnixToUtc(res.DateTimeBack),
                    Culture = "ru-RU",
                    Amount = res.Price,
                    CurrencySymbol = "$"
                };

                ocr.FlyResults.Add(ofr);
            }

            return string.Join(Environment.NewLine, dict.Values);
        }

        private async Task<OneCityResult> CreateResult(string cityCode, long chatId)
        {
            var city = await _dbContext.Set<City>().FirstAsync(x => x.Code == cityCode);

            var cityLoc = await _localizationHelper.GetEntityLocalizationForChat(chatId, city);
            var countryLoc = await _localizationHelper.GetEntityLocalizationForChat(chatId, city.Country);

            return new OneCityResult
            {
                CityLocalName = cityLoc.Localization,
                CountryLocalName = countryLoc.Localization
            };
        }
    }
}
