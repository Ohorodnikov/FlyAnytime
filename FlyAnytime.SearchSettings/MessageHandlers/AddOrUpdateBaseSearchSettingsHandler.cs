using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
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

        public AddOrUpdateBaseSearchSettingsHandler(
            IRepository<Chat> chatRepo,
            IRepository<Country> countryRepo
            )
        {
            _chatRepo = chatRepo;
            _countryRepo = countryRepo;
        }

        public async Task Handle(AddOrUpdateBaseSearchSettingsMessage message)
        {
            var cityFlyFrom = message.CityFromFlyCode;
            var countryFlyTo = message.CountryToFlyCode;

            var chatResult = await _chatRepo.GetBy(chat => chat.ChatId, message.ChatId.ToString());

            if (!chatResult.Success)
            {
                return;
            }

            var chat = chatResult.Entity;

            chat.SearchSettings ??= new List<ChatSearchSettings>();

            var settingsCount = chat.SearchSettings.Where(x => x.IsActive).Count();

            if (settingsCount == 0)
            {
                var defCurr = await GetDefaultCountryCurrency(message.CountryFromFlyCode);
                var defPriceSett = CreatePriceSettings(message.PriceMax, defCurr);
                var flyTo = await _countryRepo.GetBy(x => x.Code, message.CountryToFlyCode);

                var set = GetDefaultSettings(defPriceSett, flyTo.Entity);

                var resList = chat.SearchSettings.ToList();

                resList.Add(set);

                chat.SearchSettings = resList;

                var updateRes = await _chatRepo.TryReplace(chat);
            }
            else if (settingsCount == 1)
            {

            }
            else
            {
                return;
            }
        }

        private async Task<string> GetDefaultCountryCurrency(string countryCode)
        {
            var country = await _countryRepo.GetBy(x => x.Code, countryCode);

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

        private ChatSearchSettings GetDefaultSettings(PriceSettings price, Country flyTo)
        {
            var ssNew = new ChatSearchSettings
            {
                IsActive = true,
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
                        Code = "Created from tg settings",
                        Countries = new List<Country>
                        {
                            flyTo,
                        }
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
