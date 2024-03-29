﻿using System;

namespace FlyAnytime.Messaging.Messages.Scheduler
{
    public struct FixedSearchTimeFrame
    {
        public FixedSearchTimeFrame(long startDate, long endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public long StartDate { get; }
        public long EndDate { get; }
    }

    public class CreateFixedDateSearchJobMessage : CreateSearchJobMessage<FixedSearchTimeFrame>
    {
        public CreateFixedDateSearchJobMessage(long chatId,
                                               Guid settingsId,
                                               FlyDirection flyDirection,
                                               PriceSettings priceSettings,
                                               TripDuration tripDuration,
                                               ScheduleSettings scheduleSettings,
                                               FixedSearchTimeFrame searchTimeFrame) 
                                                : base(chatId, settingsId, flyDirection, priceSettings, tripDuration, scheduleSettings, searchTimeFrame)
        {
        }
    }
}
