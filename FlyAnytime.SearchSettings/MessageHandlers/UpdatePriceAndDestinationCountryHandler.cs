using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.SearchSettings.Helpers;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.Models.SearchSettings;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.Repository;
using FlyAnytime.Tools;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MessageHandlers
{
    public class UpdatePriceAndDestinationCountryHandler : IMessageHandler<UpdatePriceAndDestinationCountryMessage>
    {
        private readonly IRepository<Chat> _chatRepo;
        private readonly IRepository<Country> _countryRepo;
        private readonly IRepository<City> _cityRepo;
        private readonly IRepository<Airport> _airRepo;
        private readonly IPublishEditChatSettingsHelper _eventHelper;

        public UpdatePriceAndDestinationCountryHandler(
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

        public async Task Handle(UpdatePriceAndDestinationCountryMessage message)
        {
            var chatResult = await _chatRepo.GetOneBy(chat => chat.ChatId == message.ChatId);

            if (!chatResult.Success)
                return;

            var chat = chatResult.Entity;

            chat.SearchSettings ??= Enumerable.Empty<ChatSearchSettings>();

            var settingsActive = chat.SearchSettings.Where(x => x.IsActive).ToList();

            if (settingsActive.Count > 1)
                return;

            if (settingsActive.Count == 1)
            {
                var def = settingsActive.FirstOrDefault(x => x.Title == "Default");

                if (def == null)
                    return;

                await _eventHelper.SendDeleteSettingsEvent(chat.ChatId, new[] { def });
            }

            var defSet = await CreateDefaultSearchSettings(chat, message);

            SetDefSettings(chat, defSet);

            var updateRes = await _chatRepo.TryReplace(chat);

            if (updateRes.Success)
                await _eventHelper.SendUpdatedSettingsEvent(updateRes.Entity);
        }

        private void SetDefSettings(Chat chat, ChatSearchSettings settings)
        {
            chat.SearchSettings = chat.SearchSettings.Where(x => x.Title != settings.Title).Append(settings);
        }

        private async Task<ChatSearchSettings> CreateDefaultSearchSettings(Chat chat, UpdatePriceAndDestinationCountryMessage message)
        {   
            var defPriceSett = CreatePriceSettings(message.PriceMax);
            var flyTo = await _countryRepo.GetOneBy(x => x.Code == message.CountryToFlyCode);

            return await GetDefaultSettings(defPriceSett, flyTo.Entity);
        }

        private PriceSettings CreatePriceSettings(decimal amount)
        {
            return new PriceSettings
            {
                Type = SearchPriceSettingsType.FixPrice,
                Amount = amount,
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
