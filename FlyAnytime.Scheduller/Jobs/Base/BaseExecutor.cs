using FlyAnytime.Core.Entity;
using FlyAnytime.Scheduler.EF;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.Jobs.Base
{
    public abstract class BaseExecutor<TJobData> : IJob
        where TJobData : class, IEntity
    {
        SchedulerDbContext _context;
        public BaseExecutor(SchedulerDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ctor = typeof(TJobData).GetConstructor(new[] { typeof(JobDataMap) });

            var data = new SearchBySettingsJobData(context.JobDetail.JobDataMap);

           var jobData = await _context.Set<TJobData>().FindAsync(data.JobDataId);

            await Execute(context, jobData);
        }

        protected abstract Task Execute(IJobExecutionContext context, TJobData data);
    }
}
