using FlyAnytime.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyAnytime.Messaging.Messages.Scheduler
{
    public struct DynamicSearchTimeFrame
    {
        public DynamicSearchTimeFrame(int daysFromNowStart, int daysFromNowEnd)
        {
            DaysFromNowStart = daysFromNowStart;
            DaysFromNowEnd = daysFromNowEnd;
        }

        public int DaysFromNowStart { get; }
        public int DaysFromNowEnd { get; }
    }

    public class CreateDynamicDateSearchJobMessage : CreateSearchJobMessage<DynamicSearchTimeFrame>
    {
        public CreateDynamicDateSearchJobMessage(
                                                long chatId,
                                                FlyDirection flyDirection,
                                                PriceSettings priceSettings,
                                                TripDuration tripDuration,
                                                ScheduleSettings schedule,
                                                DynamicSearchTimeFrame searchTimeFrame,
                                                
                                                Dictionary<Days, HashSet<byte>> allowedDateTimeSlotsTo,
                                                Dictionary<Days, HashSet<byte>> allowedDateTimeSlotsBack)
                                                : base(chatId, flyDirection, priceSettings, tripDuration, schedule, searchTimeFrame)
        {
            ValidateDateTimeSlots(allowedDateTimeSlotsTo);
            ValidateDateTimeSlots(allowedDateTimeSlotsBack);

            AllowedDateTimeSlotsTo = allowedDateTimeSlotsTo;
            AllowedDateTimeSlotsBack = allowedDateTimeSlotsBack;
        }

        public Dictionary<Days, HashSet<byte>> AllowedDateTimeSlotsTo { get; private set; }
        public Dictionary<Days, HashSet<byte>> AllowedDateTimeSlotsBack { get; private set; }

        private void ValidateDateTimeSlots(Dictionary<Days, HashSet<byte>> dateTimeSlots)
        {
            foreach (var slot in dateTimeSlots)
            {
                var times = slot.Value;

                var valid = times.All(x => x >= 0 && x <= 23);

                if (!valid)
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
