using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Jobs.Base;
using FlyAnytime.Scheduler.Models;
using FlyAnytime.Tools;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.Jobs
{
    public class DynamicDateSearchJob : BaseExecutor<DynamicDateSearchJobData>
    {
        private readonly IMessageBus _messageBus;
        public DynamicDateSearchJob(SchedulerDbContext context, IMessageBus messageBus) : base(context)
        {
            _messageBus = messageBus;
        }

        protected override async Task Execute(IJobExecutionContext context, DynamicDateSearchJobData data)
        {
            var pSet = new PriceSettings(data.PriceType, data.PriceAmount, data.Currency);
            var tripDur = new TripDuration(data.TripDaysCountMin, data.TripDaysCountMax);

            var frameStart = DateTime.Now.AddDays(data.DaysFromNowStart).ToUtcUnix();
            var frameEnd = DateTime.Now.AddDays(data.DaysFromNowEnd).ToUtcUnix();

            var startSlots = data.GetAllowedDateTimeSlotsTo();
            var backSlots = data.GetAllowedDateTimeSlotsBack();

            var sFrame = new SearchFrame(frameStart, frameEnd, startSlots, backSlots);

            var msg = new MakeSearchMessage(
                data.ChatId,
                data.CityFlyFrom,
                data.AirportsFlyTo.Split(";"),
                pSet,
                tripDur,
                sFrame
                );

            _messageBus.Publish(msg);
            //throw new NotImplementedException();
        }
    }
}
