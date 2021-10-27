using FlyAnytime.Core;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Helpers;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.SearchSettings.Helpers;
using FlyAnytime.SearchSettings.MessageHandlers;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using FlyAnytime.SearchSettings.Repository;
using FlyAnytime.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLazy<ICommonSettings, CommonSettings>(services.AddSingleton);
            services.Configure<MongoSettings>(Configuration.GetSection(nameof(MongoSettings)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoSettings>>().Value);

            services.AddAllImplementations<IMongoEntity>(services.AddTransient);

            services.AddAllImplementationsWithAllInterfaces<IMongoEntityMap>(services.AddSingleton);

            services.AddTransient<IMongoDbContext, MongoDbContext>();
            services.AddScoped<IDbContextBase, MongoDbContext>();

            services.AddAllGenericImplementations(typeof(IValidator<>), services.AddTransient);
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            services.AddTransient<IPublishEditChatSettingsHelper, PublishEditChatSettingsHelper>();
            services.AddTransient<IChatSettingsHelper, ChatSettingsHelper>();

            services.AddAutoMapper(typeof(Startup));

            services.AddRabbitMq();
            services.AddControllers()
                .AddNewtonsoftJson()
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.GetService<IMongoDbContext>().DoMap();//.Wait();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

            SubscribeOnMessages(app);
        }

        private void SubscribeOnMessages(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

            eventBus.Subscribe<AppInitMessage, AppInitMessageHandler>();
            eventBus.Subscribe<ReCreateDbMessage, ReCreateDbMessageHandler>();

            eventBus.Subscribe<RegisterNewChatMessage, RegisterNewChatHandler>();
            eventBus.Subscribe<AddOrUpdateBaseSearchSettingsMessage, AddOrUpdateBaseSearchSettingsHandler>();
        }
    }
}
