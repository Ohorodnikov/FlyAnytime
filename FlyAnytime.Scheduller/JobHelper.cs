using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler
{
    public interface IJobHelper
    {
        string CreateSearchJobGroupName(long chatId, Guid settingsId);
        Task DeleteAllJobsForChat(long chatId);
        Task DeleteJobsByGroup(string groupName);
        Task DeleteJobsForChatSettings(long chatId, Guid settingsId);
        Task PauseAllJobsForChat(long chatId);
        Task PauseJobsByGroup(string groupName);
    }

    public class JobHelper : IJobHelper
    {
        IScheduler _scheduler;
        SchedulerDbContext _dbContext;
        public JobHelper(IScheduler scheduler, SchedulerDbContext dbContext)
        {
            _scheduler = scheduler;
            _dbContext = dbContext;
        }

        public string CreateSearchJobGroupName(long chatId, Guid settingsId)
        {
            return $"{chatId}.{settingsId.ToString("N")}";
        }

        public async Task DeleteJobsByGroup(string groupName)
        {
            var groups = GroupMatcher<JobKey>.GroupEquals(groupName);
            var keys = await _scheduler.GetJobKeys(groups);

            await _scheduler.DeleteJobs(keys);
        }

        //public async Task DeleteTrigersByGroup(string groupName)
        //{
        //    await _scheduler.Trig
        //}

        public async Task DeleteJobsForChatSettings(long chatId, Guid settingsId)
        {
            var groupName = CreateSearchJobGroupName(chatId, settingsId);

            await DeleteJobsByGroup(groupName);

            await DeleteJobData<FixedDateSearchJobData>(chatId, settingsId);
            await DeleteJobData<DynamicDateSearchJobData>(chatId, settingsId);
        }

        private async Task DeleteJobData<TJobData>(long chatId, Guid? settingsId = null)
            where TJobData : BaseSearchJobData
        {
            Expression<Func<TJobData, bool>> expr;

            if (settingsId.HasValue)
            {
                var val = settingsId.Value;
                expr = data => data.ChatId == chatId && data.SettingsId == val;
            }
            else
            {
                expr = data => data.ChatId == chatId;
            }

            var datas = await _dbContext.Set<TJobData>().Where(expr).ToListAsync();

            _dbContext.RemoveRange(datas);

            _dbContext.SaveChanges();
        }

        public async Task DeleteAllJobsForChat(long chatId)
        {
            var groups = GroupMatcher<JobKey>.GroupStartsWith($"{chatId}.");
            var keys = await _scheduler.GetJobKeys(groups);

            await _scheduler.DeleteJobs(keys);

            await DeleteJobData<FixedDateSearchJobData>(chatId);
            await DeleteJobData<DynamicDateSearchJobData>(chatId);
        }

        public async Task PauseJobsByGroup(string groupName)
        {
            var groups = GroupMatcher<JobKey>.GroupEquals(groupName);

            await _scheduler.PauseJobs(groups);
        }

        public async Task PauseAllJobsForChat(long chatId)
        {
            var groups = GroupMatcher<JobKey>.GroupStartsWith($"{chatId}.");

            await _scheduler.PauseJobs(groups);
        }
    }
}
