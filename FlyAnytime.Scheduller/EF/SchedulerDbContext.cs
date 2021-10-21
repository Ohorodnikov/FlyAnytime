using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Scheduler.EF
{
    public class SchedulerDbContext : BaseEfContext<SchedulerDbContext>
    {
        public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options, IServiceProvider serviceProvider) 
            : base(options, serviceProvider)
        {
        }
    }
}
