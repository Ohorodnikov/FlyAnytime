using FlyAnytime.Core;
using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Helpers;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.Telegram.Bot;
using FlyAnytime.Telegram.Bot.Commands;
using FlyAnytime.Telegram.Bot.Conversations;
using FlyAnytime.Telegram.Bot.InlineKeyboardButtons;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.MessageHandlers;
using FlyAnytime.Telegram.Models;
using FlyAnytime.Telegram.Services;
using FlyAnytime.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FlyAnytime.Telegram
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        public IConfiguration Configuration { get; }
        private BotConfiguration BotConfig { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // There are several strategies for completing asynchronous tasks during startup.
            // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            // We are going to use IHostedService to add and later remove Webhook
            services.AddSingleton<TgWebhook>();

            // Register named HttpClient to get benefits of IHttpClientFactory
            // and consume it with ITelegramBotClient typed client.
            // More read:
            //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
            //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient("tgwebhook")
                    .AddTypedClient<ITelegramBotClient>(httpClient
                        => new TelegramBotClient(BotConfig.BotToken, httpClient));


            services.AddLazy<ICommonSettings, CommonSettings>(services.AddSingleton);
            services.AddAllImplementations<IBotCommand>(services.AddScoped);
            services.AddAllImplementations<IInlineKeyboardButtonWithAction>(services.AddScoped);
            services.AddAllImplementations<IConversation>(services.AddScoped);

            services.AddIEntityAsBase();

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<TelegramContext>(opt => opt.UseLazyLoadingProxies().UseSqlServer(connection));
            services.AddScoped<IDbContextBase, TelegramContext>();

            services.AddTransient<BotClient, BotClient>();
            services.AddTransient<IBotHelper, BotHelper>();

            services.AddTransient<ILocalizationHelper, LocalizationHelper>();

            services.AddRabbitMq();

            services.AddControllers()
                .AddNewtonsoftJson()
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetRequiredService<IWebhook>().StopAsync().GetAwaiter().GetResult();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                var token = BotConfig.BotToken;
                endpoints.MapControllerRoute(name: "tgwebhook",
                                             pattern: $"bot/{token}",
                                             new { controller = "BotUpdate", action = "GetBotUpdate" });

                endpoints.MapControllers();
            });

            SubscribeOnMessages(app);
        }

        private void SubscribeOnMessages(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

            eventBus.Subscribe<AppInitMessage, AppInitMessageHandler>();
            eventBus.Subscribe<ReCreateDbMessage, ReCreateDbMessageHandler>();
            eventBus.Subscribe<DeleteAllUsersDataMessage, DeleteAllUsersDataHandler>();

            eventBus.Subscribe<AddNewLanguageMessage, AddNewLanguageHandler>();
            eventBus.Subscribe<AddOrUpdateCityMessage, AddOrUpdateCityHandler>();
            eventBus.Subscribe<AddOrUpdateCountryMessage, AddOrUpdateCountryHandler>();
            eventBus.Subscribe<DeleteCityMessage, DeleteCityHandler>();
            eventBus.Subscribe<DeleteCountryMessage, DeleteCountryHandler>();
            eventBus.Subscribe<ChangeChatCurrencyMessage, ChangeChatCurrencyHandler>();

            eventBus.Subscribe<SearchResultMessage, GetResultsMessageHandler>();
            eventBus.Subscribe<ErrorDuringSearchMessage, ErrorDuringSearchHandler>();            
        }
    }
}
