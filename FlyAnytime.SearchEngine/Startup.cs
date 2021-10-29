using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Helpers;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SearchEngine.Engine;
using SearchEngine.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
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

            //services.AddIEntityAsBase();

            services.AddTransient<ISearchEngine, Engine.SearchEngine>();

            string connection = Configuration.GetConnectionString("DefaultConnection");
            //services.AddDbContext<SchedulerDbContext>(opt => opt.UseSqlServer(connection));
            //services.AddScoped<IDbContextBase, SchedulerDbContext>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

            eventBus.Subscribe<AppInitMessage, AppInitMessageHandler>();
            eventBus.Subscribe<ReCreateDbMessage, ReCreateDbMessageHandler>();

            eventBus.Subscribe<MakeSearchMessage, MakeSearchMessageHandler>();

            //eventBus.Subscribe<CreateDynamicDateSearchJobMessage, CreateDynamicDateSearchJobHandler>();
            //eventBus.Subscribe<CreateFixedDateSearchJobMessage, CreateFixedDateSearchJobHandler>();
        }
    }
}
