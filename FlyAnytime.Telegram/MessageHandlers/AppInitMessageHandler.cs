using FlyAnytime.Core;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Telegram.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class AppInitMessageHandler : IMessageHandler<AppInitMessage>
    {
        private readonly ICommonSettings _settings;
        TgWebhook _tgWebhook;
        public AppInitMessageHandler(ICommonSettings settings, TgWebhook tgWebhook)
        {
            _settings = settings;
            _tgWebhook = tgWebhook;
        }

        public async Task Handle(AppInitMessage message)
        {
            _settings.ApiGatewayUrl = message.GatewayUrl;

            await _tgWebhook.StartAsync();
        }
    }
}
