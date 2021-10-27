using FlyAnytime.Core;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Helpers;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.MessageHandlers;
using FlyAnytime.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Scheduler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRabbitMq();

            services.AddIEntityAsBase();

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SchedulerDbContext>(opt => opt.UseSqlServer(connection));
            services.AddScoped<IDbContextBase, SchedulerDbContext>();

            services.AddTransient<IScheduler>(sp => sp.GetService<ISchedulerFactory>().GetScheduler().GetAwaiter().GetResult());

            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Core";
                q.UseMicrosoftDependencyInjectionJobFactory();

                q.UseSimpleTypeLoader();

                q.UsePersistentStore(opt =>
                {
                    //opt.Use
                    opt.UseSqlServer(connection);
                    //opt.
                    opt.UseJsonSerializer();
                });


                //q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 100;
                });
            });

            AddQuartzAsHosted(services, options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        }

        private void AddQuartzAsHosted(IServiceCollection services, Action<QuartzHostedServiceOptions> configure = null)
        {
            if (configure != null)
                services.Configure(configure);            

            services.AddSingleton<IHostedService, QuartzHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

            eventBus.Subscribe<AppInitMessage, AppInitMessageHandler>();
            eventBus.Subscribe<ReCreateDbMessage, ReCreateDbMessageHandler>();

            eventBus.Subscribe<CreateDynamicDateSearchJobMessage, CreateDynamicDateSearchJobHandler>();
            eventBus.Subscribe<CreateFixedDateSearchJobMessage, CreateFixedDateSearchJobHandler>();
        }
    }
}
