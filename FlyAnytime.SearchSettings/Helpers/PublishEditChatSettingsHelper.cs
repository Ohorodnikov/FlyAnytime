using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.Models.SearchSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Helpers
{
    public interface IPublishEditChatSettingsHelper
    {
        Task SendUpdatedSettingsEvent(Chat chat);
    }

    public class PublishEditChatSettingsHelper : IPublishEditChatSettingsHelper
    {
        IMessageBus _messageBus;
        IChatSettingsHelper _settingsHelper;
        public PublishEditChatSettingsHelper(
            IMessageBus messageBus,
            IChatSettingsHelper settingsHelper)
        {
            _messageBus = messageBus;
            _settingsHelper = settingsHelper;
        }

        public async Task SendUpdatedSettingsEvent(Chat chat)
        {
            var settings = chat.SearchSettings;
            var flyFrom = chat.FlyFrom;
            var chatId = chat.ChatId;

            foreach (var s in settings)
                await ProcessOneSearchSetting(chatId, flyFrom, s);
        }

        private async Task ProcessOneSearchSetting(long chatId, City flyFrom, ChatSearchSettings settings)
        {
            var airports = await _settingsHelper
                            .GetDestinationAirports(settings)
                            ;

            var airCodes = string.Empty;
            if (airports.Any())
            {
                airCodes = airports
                            .Select(x => x.Code)
                            .Aggregate((f, s) => $"{f};{s}");
            }
            

            var ps = settings.PriceSettings;
            var ds = settings.DateSettings;

            var flyDirection = new FlyDirection(flyFrom.Code, airCodes);
            var priceSettings = new Messaging.Messages.Scheduler.PriceSettings(ps.Type, ps.Amount, ps.Currency);
            var tripDuration = new TripDuration(ds.TripDaysCountMin, ds.TripDaysCountMax);
            var schedules = settings.Schedules.Select(x => _settingsHelper.CreateScheduleSettings(x));

            var msgs = settings.DateSettings.Type switch
            {
                SearchDateSettingsType.FixedRange => GetFixedDateSearchMsgs(chatId, flyDirection, priceSettings, tripDuration, schedules, ds.FixedDateSettings),
                SearchDateSettingsType.DynamicRange => GetDynamicDateSearchMsgs(chatId, flyDirection, priceSettings, tripDuration, schedules, ds.DynamicDateSettings),
                _ => Enumerable.Empty<BaseMessage>(),
            };

            foreach (var msg in msgs)
                _messageBus.Publish(msg);            
        }

        private IEnumerable<BaseMessage> GetDynamicDateSearchMsgs(
            long chatId, 
            FlyDirection direction,
            Messaging.Messages.Scheduler.PriceSettings priceSettings,
            TripDuration duration,
            IEnumerable<ScheduleSettings> schedules,
            DynamicDateSettings ds)
        {           
            var searchFrame = new DynamicSearchTimeFrame(ds.DaysFromNowStart, ds.DaysFromNowEnd);
            var departureDTSlots = _settingsHelper.GetDayTimeSlots(ds.DepartureFlySettings);
            var returnDTSlots = _settingsHelper.GetDayTimeSlots(ds.ReturnFlySettings);

            foreach (var schedule in schedules)
            {
                yield return new CreateDynamicDateSearchJobMessage(
                                                                chatId, 
                                                                direction, 
                                                                priceSettings, 
                                                                duration, 
                                                                schedule, 
                                                                searchFrame, 
                                                                departureDTSlots, 
                                                                returnDTSlots);
            }
        }

        private IEnumerable<BaseMessage> GetFixedDateSearchMsgs(
            long chatId,
            FlyDirection direction,
            Messaging.Messages.Scheduler.PriceSettings priceSettings,
            TripDuration duration,
            IEnumerable<ScheduleSettings> schedules,
            FixedDateSettings ds)
        {
            var dsFrame = new FixedSearchTimeFrame(ds.StartDateUtc, ds.EndDateUtc);
            foreach (var schedule in schedules)
            {
                yield return new CreateFixedDateSearchJobMessage(
                                                                chatId,
                                                                direction,
                                                                priceSettings,
                                                                duration,
                                                                schedule,
                                                                dsFrame
                                                                );
            }
        }
    }
}
