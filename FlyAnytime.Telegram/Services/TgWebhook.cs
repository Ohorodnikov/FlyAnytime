using FlyAnytime.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FlyAnytime.Telegram.Services
{
    public interface IWebhook
    {
        Task StartAsync(/*CancellationToken cancellationToken*/);
        Task StopAsync(/*CancellationToken cancellationToken*/);
    }

    public class TgWebhook : IWebhook
    {
        private readonly ILogger<TgWebhook> _logger;
        private readonly IServiceProvider _services;
        private readonly BotConfiguration _botConfig;
        private readonly Lazy<ICommonSettings> _settings;
        public TgWebhook(ILogger<TgWebhook> logger,
                                IServiceProvider serviceProvider,
                                IConfiguration configuration,
                                Lazy<ICommonSettings> settings)
        {
            _logger = logger;
            _services = serviceProvider;
            _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
            _settings = settings;
        }

        public async Task StartAsync(/*CancellationToken cancellationToken*/)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webhookAddress = @$"{_settings.Value.ApiGatewayUrl}/tgbot/bot/{_botConfig.BotToken}";
            _logger.LogInformation("Setting webhook: {0}", webhookAddress);

            await botClient.SetWebhookAsync(url: webhookAddress);
        }

        public async Task StopAsync(/*CancellationToken cancellationToken*/)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook upon app shutdown
            _logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(/*cancellationToken: cancellationToken*/);
        }
    }
}
