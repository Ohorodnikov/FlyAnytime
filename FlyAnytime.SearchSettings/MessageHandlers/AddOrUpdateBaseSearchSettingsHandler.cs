using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.SearchSettings.Helpers;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.Models.SearchSettings;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.Repository;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MessageHandlers
{
    public class AddOrUpdateBaseSearchSettingsHandler : IMessageHandler<AddOrUpdateBaseSearchSettingsMessage>
    {
        IRepository<Chat> _chatRepo;
        IRepository<Country> _countryRepo;
        IRepository<City> _cityRepo;
        IRepository<Airport> _airRepo;
        IPublishEditChatSettingsHelper _eventHelper;

        public AddOrUpdateBaseSearchSettingsHandler(
            IRepository<Chat> chatRepo,
            IRepository<Country> countryRepo,
            IRepository<City> cityRepo,
            IRepository<Airport> airRepo,
            IPublishEditChatSettingsHelper eventHelper
            )
        {
            _chatRepo = chatRepo;
            _countryRepo = countryRepo;
            _cityRepo = cityRepo;
            _airRepo = airRepo;
            _eventHelper = eventHelper;
        }

        public async Task Handle(AddOrUpdateBaseSearchSettingsMessage message)
        {
            var cityFlyFrom = message.CityFromFlyCode;
            var countryFlyTo = message.CountryToFlyCode;

            var chatResult = await _chatRepo.GetOneBy(chat => chat.ChatId == message.ChatId);

            if (!chatResult.Success)
                return;

            var chat = chatResult.Entity;

            chat.SearchSettings ??= new List<ChatSearchSettings>();

            var settingsActive = chat.SearchSettings.Where(x => x.IsActive).ToList();

            async Task AddDef()
            {
                var defCurr = await GetDefaultCountryCurrency(message.CountryFromFlyCode);
                var defPriceSett = CreatePriceSettings(message.PriceMax, defCurr);
                var flyTo = await _countryRepo.GetOneBy(x => x.Code == message.CountryToFlyCode);

                var set = await GetDefaultSettings(defPriceSett, flyTo.Entity);

                var resList = chat.SearchSettings.ToList();

                resList.Add(set);

                chat.SearchSettings = resList;

                var flyFrom = await _cityRepo.GetOneBy(x => x.Code == message.CityFromFlyCode);

                chat.FlyFrom = flyFrom.Entity;

                var updateRes = await _chatRepo.TryReplace(chat);

                if (updateRes.Success)
                {
                    await _eventHelper.SendUpdatedSettingsEvent(updateRes.Entity);
                }
            }

            if (settingsActive.Count == 0)
            {
                await AddDef();
            }
            else if (settingsActive.Count == 1)
            {
                var def = settingsActive.FirstOrDefault(x => x.Title == "Default");
                if (def != null)
                    chat.SearchSettings = chat.SearchSettings.Where(x => x.Id != def.Id);

                await AddDef();
            }
            else
            {
                return;
            }
        }

        private async Task<string> GetDefaultCountryCurrency(string countryCode)
        {
            var country = await _countryRepo.GetOneBy(x => x.Code == countryCode);

            return country.Entity.DefSearchCurrencyCode;
        }

        private PriceSettings CreatePriceSettings(decimal amount, string currency)
        {
            return new PriceSettings
            {
                Type = SearchPriceSettingsType.FixPrice,
                Amount = amount,
                Currency = currency
            };
        }

        private async Task<ChatSearchSettings> GetDefaultSettings(PriceSettings price, Country flyTo)
        {
            var cities = await _cityRepo.Where(x => x.CountryId == flyTo.Id);

            var airs = new List<Airport>();

            foreach (var city in cities)
            {
                var cityAirs = await _airRepo.Where(x => x.CityId == city.Id);
                airs.AddRange(cityAirs);
            }

            var ssNew = new ChatSearchSettings
            {
                IsActive = true,
                Title = "Default",
                DateSettings = new DateSettings
                {
                    Type = SearchDateSettingsType.DynamicRange,
                    TripDaysCountMin = 3,
                    TripDaysCountMax = 7,
                    DynamicDateSettings = new DynamicDateSettings
                    {
                        DaysFromNowStart = 30,
                        DaysFromNowEnd = 30 * 6,
                        DepartureFlySettings = AllWeek(),
                        ReturnFlySettings = AllWeek()
                    }
                },
                Schedules = new List<Schedule>
                {
                    new Schedule
                    {
                        Type = ScheduleIntervalType.Day,
                        Interval = 1,
                        Hour = 8,
                        Minute = 0
                    }
                },
                PriceSettings = price,
                SearchGroups = new List<ChatSearchGroup>
                {
                    new ChatSearchGroup
                    {
                        IsActive = true,
                        Code = $"All airports from {flyTo.Name}",
                        Airports = airs
                    }
                }
            };

            return ssNew;
        }

        private List<FlyDaySettings> AllWeek()
        {
            FlyDaySettings GetForDay(Days day)
            {
                return new FlyDaySettings
                {
                    Day = day,
                    AllowedHourStart = 0,
                    AllowedHourEnd = 23
                };
            }

            return new List<FlyDaySettings>
            {
                GetForDay(Days.Monday),
                GetForDay(Days.Tuesday),
                GetForDay(Days.Wednesday),
                GetForDay(Days.Thursday),
                GetForDay(Days.Friday),
                GetForDay(Days.Saturday),
                GetForDay(Days.Sunday),
            };
        }
    }
}
