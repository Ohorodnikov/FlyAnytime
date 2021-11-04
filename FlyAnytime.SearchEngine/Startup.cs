using FlyAnytime.Core;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Helpers;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.SearchEngine;
using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Engine.ApiRequesters;
using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi;
using FlyAnytime.SearchEngine.MessageHandlers;
using FlyAnytime.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FlyAnytime.SearchEngine.Engine;
using FlyAnytime.SearchEngine.MessageHandlers;
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

            services.AddIEntityAsBase();

            services.AddTransient<ISearchEngine, FlyAnytime.SearchEngine.Engine.SearchEngine>();

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SearchEngineContext>(opt => opt.UseSqlServer(connection));
            services.AddScoped<IDbContextBase, SearchEngineContext>();

            services.AddMemoryCache();

            services.AddChannel<ApiResultModel>();
            services.AddHostedService<SaveSearchResultBackgroundService>();

            services.AddTransient<IApiRequester, KiwiSearchApi>();
            services.AddScoped<ICacheHelper, CacheHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

            eventBus.Subscribe<AppInitMessage, AppInitMessageHandler>();
            eventBus.Subscribe<ReCreateDbMessage, ReCreateDbMessageHandler>();

            eventBus.Subscribe<MakeSearchMessage, MakeSearchMessageHandler>();

            eventBus.Subscribe<AddOrUpdateCityMessage, AddCityMessageHandler>();
            eventBus.Subscribe<AddOrUpdateAirportMessage, AddAirportMessageHandler>();

            //eventBus.Subscribe<CreateDynamicDateSearchJobMessage, CreateDynamicDateSearchJobHandler>();
            //eventBus.Subscribe<CreateFixedDateSearchJobMessage, CreateFixedDateSearchJobHandler>();
        }
    }
}
