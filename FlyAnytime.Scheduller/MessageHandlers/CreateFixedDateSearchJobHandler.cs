using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Jobs;
using FlyAnytime.Scheduler.Models;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public class CreateFixedDateSearchJobHandler : BaseCreateSearchJobHandler<CreateFixedDateSearchJobMessage, FixedDateSearchJob, FixedDateSearchJobData>
    {
        public CreateFixedDateSearchJobHandler(IScheduler scheduler, SchedulerDbContext dbContext, ILogger<CreateFixedDateSearchJobHandler> logger) 
            : base(scheduler, dbContext, logger) { }

        protected override FixedDateSearchJobData ConvertMessage2Data(CreateFixedDateSearchJobMessage message)
        {
            return new FixedDateSearchJobData
            {
                ChatId = message.ChatId,
                CityFlyFrom = message.FlyDirection.CityFlyFrom,
                AirportsFlyTo = message.FlyDirection.AirportsFlyTo,
                Currency = message.PriceSettings.Currency,
                PriceType = message.PriceSettings.Type,
                PriceAmount = message.PriceSettings.Amount,
                TripDaysCountMin = message.TripDuration.DaysMin,
                TripDaysCountMax = message.TripDuration.DaysMax,

                StartDate = message.SearchTimeFrame.StartDate,
                EndDate = message.SearchTimeFrame.EndDate
            };
        }
    }
}
