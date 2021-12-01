using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Jobs;
using FlyAnytime.Scheduler.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public abstract class BaseCreateSearchJobHandler<TMessage, TJob, TData> : IMessageHandler<TMessage>
        where TMessage : CreateSearchJobMessage
        where TJob : IJob
        where TData : BaseSearchJobData
    {
        private readonly IScheduler _scheduler;
        private readonly SchedulerDbContext _dbContext;
        private readonly IJobHelper _jobHelper;
        private readonly ILogger _logger;
        public BaseCreateSearchJobHandler(IScheduler scheduler,
                                          SchedulerDbContext dbContext,
                                          IJobHelper jobHelper,
                                          ILogger<BaseCreateSearchJobHandler<TMessage, TJob, TData>> logger)
        {
            _scheduler = scheduler;
            _dbContext = dbContext;
            _jobHelper = jobHelper;
            _logger = logger;
        }


        public async Task Handle(TMessage message)
        {
            var jobData = await SaveJobData(message);

            await CreateJob(message, jobData.Id);
        }

        protected async Task<TData> SaveJobData(TMessage message)
        {
            var data = ConvertMessage2Data(message);
            _dbContext.Add(data);

            await _dbContext.SaveChangesAsync();

            return data;
        }

        protected abstract TData ConvertMessage2Data(TMessage message);

        private async Task CreateJob(TMessage message, long jobDataId)
        {
            var data = new SearchBySettingsJobData
            {
                JobDataId = jobDataId
            };

            var groupName = _jobHelper.CreateSearchJobGroupName(message.ChatId, message.SettingsId);

            var job = JobBuilder
                                .Create<TJob>()
                                .WithIdentity(jobDataId.ToString(), groupName)
                                .UsingJobData(data)
                                .Build()
                                ;            

            var trigger = TriggerBuilder
                                        .Create()
                                        .WithIdentity(message.Schedule.Id, groupName)
                                        .WithCronSchedule(GetCronScheduleString(message.Schedule))
                                        .Build()
                                        ;

            _logger.LogInformation($"Save job for chat {message.ChatId}. Next fire time utc: {trigger.GetNextFireTimeUtc()}");
            await _scheduler.ScheduleJob(job, trigger);
        }

        private string GetCronScheduleString(ScheduleSettings set)
        {
            return $"0 {GetNextMinute()}/3 * * * ?";
            var cronExpr = set.IntervalType switch
            {
                ScheduleIntervalType.Hour => $"0 {GetNextMinute()} 0/{set.IntervalValue} 1/1 * ? *",
                ScheduleIntervalType.Day => $"0 {set.Minute} {set.Hour} 1/{set.IntervalValue} * ? *",//Every set.IntervalValue days at 0 sec set.Minute set.Hour starting from 1 day of Month
                ScheduleIntervalType.Custom => set.CustomSchedule,
                _ => throw new NotImplementedException(),
            };

            return cronExpr;
        }

        private static int _nextMinute = 0;
        private static int GetNextMinute()
        {
            var res = Interlocked.Increment(ref _nextMinute);
            Interlocked.CompareExchange(ref _nextMinute, 0, int.MaxValue);

            return res % 59;
        }
    }
}
