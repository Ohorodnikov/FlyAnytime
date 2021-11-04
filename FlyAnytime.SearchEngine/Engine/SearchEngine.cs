using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.SearchEngine;
using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Engine;
using FlyAnytime.SearchEngine.Engine.ApiRequesters;
using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels;
using FlyAnytime.SearchEngine.Exceptions;
using FlyAnytime.SearchEngine.Models.DbModels;
using FlyAnytime.Tools;
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.Engine
{
    public interface ISearchEngine
    {
        Task<IEnumerable<OneResult>> Search(MakeSearchMessage settings); 
    }

    public class SearchEngine : ISearchEngine
    {
        private readonly IApiRequester _apiRequester;
        private readonly ICacheHelper _cacheHelper;
        private readonly ChannelWriter<ApiResultModel> _channel;
        public SearchEngine(IApiRequester apiRequester, ICacheHelper cacheHelper, ChannelWriter<ApiResultModel> channel)
        {
            _apiRequester = apiRequester;
            _cacheHelper = cacheHelper;
            _channel = channel;
        }

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

        private async Task<HashSet<string>> GetCitiesForAirports(IEnumerable<string> airportCodes)
        {
            var airs = airportCodes.ToList();
            var res = new HashSet<string>(airs.Count);

            foreach (var air in airs)
            {
                var city = await _cacheHelper.GetCityCodeForAirport(air);
                res.Add(city);
            }

            return res;
        }

        public async Task<IEnumerable<OneResult>> Search(MakeSearchMessage settings)
        {
            ValidateSettings(settings);

            var takeOneDirectionResults = 3;

            var flyFrom = DateTimeHelper.UnixToUtc(settings.SearchFrame.Start);
            var returnTo = DateTimeHelper.UnixToUtc(settings.SearchFrame.End);

            var cities = await GetCitiesForAirports(settings.AirportsFlyTo);

            var requests = new List<IApiRequestSender>(cities.Count);
            foreach (var city in cities)
            {
                var r = _apiRequester
                                    .CreateRequest(ApiRequestHelper.GenerateRequestGroupName(settings.CityFlyFrom, city))
                                    .SetFlyDirection(settings.CityFlyFrom, city)
                                    .AddParam(new Currency(settings.PriceSettings.Currency))
                                    .AddFlyFromAndReturnDates(flyFrom, returnTo, settings.TripDuration.DaysMin, settings.TripDuration.DaysMax)
                                    .AddPersons(2, 0, 0)
                                    .Build();

                requests.Add(r);
            }

            var results = new List<OneResult>(cities.Count*takeOneDirectionResults);

            foreach (var r in requests)
            {
                var resultTask = r.Send();
                var avTask = _cacheHelper.GetAveragePrice(r.Key);

                await Task.WhenAll(resultTask, avTask);

                var averPrice = await avTask;
                var searchRes = await resultTask;

                foreach (var res in searchRes)
                    await _channel.WriteAsync(res);

                var data2Send = FilterResults(searchRes, averPrice, settings)
                                .OrderBy(x => x.PriceInEur)
                                .Take(takeOneDirectionResults);

                results.AddRange(Convert2DisplayResults(data2Send, averPrice));
            }

            //Parallel.ForEach(requests, async r =>
            //{
            //    var resultTask = r.Send();
            //    var avTask = _cacheHelper.GetAveragePrice(r.Key);

            //    await Task.WhenAll(resultTask, avTask);

            //    var averPrice = await avTask;
            //    var searchRes = await resultTask;

            //    foreach (var res in searchRes)
            //        await _channel.WriteAsync(res);

            //    var data2Send = FilterResults(searchRes, averPrice, settings).OrderBy(x => x.PriceInEur).Take(3);

            //    results.AddRange(Convert2DisplayResults(data2Send, averPrice));
            //});

            return results;
        }

        private IEnumerable<ApiResultModel> FilterResults(IEnumerable<ApiResultModel> data, decimal averagePrice, MakeSearchMessage settings)
        {
            var slotsToStart = settings.SearchFrame.AllowedDateTimeSlotsTo;
            var slotsToBack = settings.SearchFrame.AllowedDateTimeSlotsBack;

            return data
                .Where(settings.GetFilterForPrice(averagePrice))
                .Where(d => FilterHelper.IsDateInsideAllowedSlots(d.DepartureFromDateTimeLocal, slotsToStart))
                .Where(d => FilterHelper.IsDateInsideAllowedSlots(d.ArrivalBackDateTimeLocal, slotsToBack))
                ;
        }

        private IEnumerable<OneResult> Convert2DisplayResults(IEnumerable<ApiResultModel> data, decimal averagePrice)
        {
            var is0 = averagePrice == 0;
            return data.Select(x =>
            {
                var discountValue = is0 ? 0 : ((x.Price - averagePrice) / averagePrice * 100); //if <0 then ticket is cheaper, if >0 - more expensive
                return new OneResult
                {
                    CityFrom = x.CityCodeFrom,
                    CityTo = x.CityCodeTo,
                    DateTimeFrom = x.DepartureFromDateTimeLocal,
                    DateTimeBack = x.ArrivalBackDateTimeLocal,
                    Price = x.Price,
                    ResultUrl = x.LinkOnResult,
                    DiscountPercent = discountValue
                };
            });
        }
    }
}
