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
    public class DynamicDateSearchJob : BaseExecutor<DynamicDateSearchJobData>
    {
        public DynamicDateSearchJob(SchedulerDbContext context) : base(context)
        {
        }

        protected override async Task Execute(IJobExecutionContext context, DynamicDateSearchJobData data)
        {
            throw new NotImplementedException();
        }
    }
}
