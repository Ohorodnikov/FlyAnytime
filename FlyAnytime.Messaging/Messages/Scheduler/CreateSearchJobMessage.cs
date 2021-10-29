using FlyAnytime.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Text;

namespace FlyAnytime.Messaging.Messages.Scheduler
{
    public class FlyDirection
    {
        public FlyDirection(string cityFlyFrom, string airportsFlyTo)
        {
            CityFlyFrom = cityFlyFrom;
            AirportsFlyTo = airportsFlyTo;
        }

        public string CityFlyFrom { get; private set; }
        public string AirportsFlyTo { get; private set; }
    }

    public class PriceSettings
    {
        public PriceSettings(SearchPriceSettingsType type, decimal amount, string currency)
        {
            Type = type;
            Amount = amount;
            Currency = currency;
        }

        public SearchPriceSettingsType Type { get; }
        public decimal Amount { get; }
        public string Currency { get; }
    }

    public class TripDuration
    {
        public TripDuration(int daysMin, int daysMax)
        {
            DaysMin = daysMin;
            DaysMax = daysMax;
        }

        public int DaysMin { get; }
        public int DaysMax { get; }
    }

    public class ScheduleSettings
    {
        [JsonConstructor]
        protected ScheduleSettings(
            string id, bool isActive, ScheduleIntervalType intervalType, 
            byte intervalValue, byte hour, byte minute, string customSchedule) 
            : this(id, isActive, intervalType)
        {
            IntervalValue = intervalValue;
            Hour = hour;
            Minute = minute;

            CustomSchedule = customSchedule;
        }

        private ScheduleSettings(string id, bool isActive, ScheduleIntervalType type)
        {
            IntervalType = type;
            Id = id;
            IsActive = isActive;
        }
        /// <summary>
        /// Custom Schedule only
        /// </summary>
        /// <param name="intervalType"></param>
        /// <param name="customSchedule"></param>
        public ScheduleSettings(string id, bool isActive, string customSchedule) : this(id, isActive, ScheduleIntervalType.Custom)
        {
            CustomSchedule = customSchedule;

            IntervalValue = 0;
            Hour = 0;
            Minute = 0;
        }

        public ScheduleSettings(string id, bool isActive, byte intervalHoursValue) : this(id, isActive, ScheduleIntervalType.Hour)
        {
            IntervalValue = intervalHoursValue;
        }

        public ScheduleSettings(string id, bool isActive, byte intervalDaysValue, byte dayHourAt, byte dayMinuteAt) : this(id, isActive, ScheduleIntervalType.Day)
        {
            IntervalValue = intervalDaysValue;
            Hour = dayHourAt;
            Minute = dayMinuteAt;
        }

        public string Id { get; }
        public bool IsActive { get; }
        public ScheduleIntervalType IntervalType { get; }

        public byte IntervalValue { get; }
        public byte Hour { get; }
        public byte Minute { get; }

        public string CustomSchedule { get; }
    }

    public abstract class CreateSearchJobMessage : BaseSearchJobMessage
    {
        protected CreateSearchJobMessage(long chatId, Guid settingsId, ScheduleSettings schedule) : base(chatId, settingsId)
        {
            Schedule = schedule;
        }
        public ScheduleSettings Schedule { get; private set; }
    }

    public abstract class CreateSearchJobMessage<TSearchTimeFrame> : CreateSearchJobMessage
    {
        protected CreateSearchJobMessage(long chatId,
                                         Guid settingsId,
                                         FlyDirection flyDirection,
                                         PriceSettings priceSettings,
                                         TripDuration tripDuration,
                                         ScheduleSettings schedule,
                                         TSearchTimeFrame searchTimeFrame)
                                        : base(chatId, settingsId, schedule)
        {
            FlyDirection = flyDirection;
            PriceSettings = priceSettings;
            TripDuration = tripDuration;
            SearchTimeFrame = searchTimeFrame;
        }

        public FlyDirection FlyDirection { get; private set; }

        public PriceSettings PriceSettings { get; private set; }

        public TripDuration TripDuration { get; private set; }

        public TSearchTimeFrame SearchTimeFrame { get; private set; }
    }
}
