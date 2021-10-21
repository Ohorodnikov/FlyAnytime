using FlyAnytime.Core.Enums;
using System;
using System.Text;

namespace FlyAnytime.Messaging.Messages.Scheduler
{
    public struct FlyDirection
    {
        public FlyDirection(string cityFlyFrom, string airportsFlyTo)
        {
            CityFlyFrom = cityFlyFrom;
            AirportsFlyTo = airportsFlyTo;
        }

        public string CityFlyFrom { get; private set; }
        public string AirportsFlyTo { get; private set; }
    }

    public struct PriceSettings
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

    public struct TripDuration
    {
        public TripDuration(int daysMin, int daysMax)
        {
            DaysMin = daysMin;
            DaysMax = daysMax;
        }

        public int DaysMin { get; }
        public int DaysMax { get; }
    }

    public struct ScheduleSettings
    {
        public ScheduleSettings(ScheduleIntervalType intervalType, string customSchedule)
        {
            if (intervalType != ScheduleIntervalType.Custom)
                throw new ArgumentOutOfRangeException(nameof(intervalType), "This ctor is only for interval type Day");

            CustomSchedule = customSchedule;
            IntervalType = intervalType;

            IntervalValue = 0;
            Hour = 0;
            Minute = 0;

        }

        public ScheduleSettings(ScheduleIntervalType intervalType, byte intervalValue)
        {
            if (intervalType != ScheduleIntervalType.Hour)
                throw new ArgumentOutOfRangeException(nameof(intervalType), "This ctor is only for interval type Hour");

            IntervalType = intervalType;
            IntervalValue = intervalValue;

            Hour = 0;
            Minute = 0;
            CustomSchedule = null;
        }

        public ScheduleSettings(ScheduleIntervalType intervalType, byte intervalValue, byte hour, byte minute)
        {
            if (intervalType != ScheduleIntervalType.Day)
                throw new ArgumentOutOfRangeException(nameof(intervalType), "This ctor is only for interval type Day");

            IntervalType = intervalType;
            IntervalValue = intervalValue;
            Hour = hour;
            Minute = minute;

            CustomSchedule = null;
        }

        public ScheduleIntervalType IntervalType { get; }
        public byte IntervalValue { get; }
        public byte Hour { get; }
        public byte Minute { get; }

        public string CustomSchedule { get; }
    }

    public abstract class CreateSearchJobMessage : BaseMessage
    {
        protected CreateSearchJobMessage(long chatId, ScheduleSettings schedule)
        {
            ChatId = chatId;
            Schedule = schedule;
        }

        public long ChatId { get; private set; }
        public ScheduleSettings Schedule { get; private set; }

    }
    public abstract class CreateSearchJobMessage<TSearchTimeFrame> : CreateSearchJobMessage
    {
        protected CreateSearchJobMessage(long chatId,
                                         FlyDirection flyDirection,
                                         PriceSettings priceSettings,
                                         TripDuration tripDuration,
                                         ScheduleSettings scheduleSettings,
                                         TSearchTimeFrame searchTimeFrame)
                                        : base(chatId, scheduleSettings)
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
