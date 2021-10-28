using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Jobs.Base;
using FlyAnytime.Scheduler.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.Jobs
{
    public class FixedDateSearchJob : BaseExecutor<FixedDateSearchJobData>
    {
        public FixedDateSearchJob(SchedulerDbContext context) : base(context)
        {
        }

        protected override async Task Execute(IJobExecutionContext context, FixedDateSearchJobData data)
        {
            throw new NotImplementedException();
        }
    }
}
