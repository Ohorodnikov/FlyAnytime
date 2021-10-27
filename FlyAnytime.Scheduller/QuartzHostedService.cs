using FlyAnytime.Scheduler.EF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IOptions<QuartzHostedServiceOptions> _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private IScheduler scheduler = null!;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IOptions<QuartzHostedServiceOptions> options,
            IServiceScopeFactory scopeFactory)
        {
            _schedulerFactory = schedulerFactory;
            _options = options;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<SchedulerDbContext>();
                await dbContext.CreateDbIfNotExists();
            }

            scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            if (_options.Value.StartDelay.HasValue)
            {
                await scheduler.StartDelayed(_options.Value.StartDelay.Value, cancellationToken);
            }
            else
            {
                await scheduler.Start(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (scheduler != null)
            {
                await scheduler.Shutdown(_options.Value.WaitForJobsToComplete, cancellationToken);
            }
        }
    }
}
