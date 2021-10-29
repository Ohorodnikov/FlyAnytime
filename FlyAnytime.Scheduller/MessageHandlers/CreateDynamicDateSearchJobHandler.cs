using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Jobs;
using FlyAnytime.Scheduler.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public class CreateDynamicDateSearchJobHandler : BaseCreateSearchJobHandler<CreateDynamicDateSearchJobMessage, DynamicDateSearchJob, DynamicDateSearchJobData>
    {
        public CreateDynamicDateSearchJobHandler(IScheduler scheduler, SchedulerDbContext dbContext, IJobHelper jobHelper, ILogger<CreateDynamicDateSearchJobHandler> logger) 
            : base(scheduler, dbContext, jobHelper, logger) { }

        protected override DynamicDateSearchJobData ConvertMessage2Data(CreateDynamicDateSearchJobMessage message)
        {
            return new DynamicDateSearchJobData
            {
                ChatId = message.ChatId,
                SettingsId = message.SettingsId,
                CityFlyFrom = message.FlyDirection.CityFlyFrom,
                AirportsFlyTo = message.FlyDirection.AirportsFlyTo,
                Currency = message.PriceSettings.Currency,
                PriceType = message.PriceSettings.Type,
                PriceAmount = message.PriceSettings.Amount,
                TripDaysCountMin = message.TripDuration.DaysMin,
                TripDaysCountMax = message.TripDuration.DaysMax,

                DaysFromNowStart = message.SearchTimeFrame.DaysFromNowStart,
                DaysFromNowEnd = message.SearchTimeFrame.DaysFromNowEnd,
                AllowedDateTimeSlotsTo = JsonConvert.SerializeObject(message.AllowedDateTimeSlotsTo),
                AllowedDateTimeSlotsBack = JsonConvert.SerializeObject(message.AllowedDateTimeSlotsBack),
            };
        }
    }
}
