﻿using FlyAnytime.Core;
using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MessageHandlers
{
    public class AppInitMessageHandler : IMessageHandler<AppInitMessage>
    {
        private readonly ICommonSettings _settings;
        public AppInitMessageHandler(ICommonSettings settings)
        {
            _settings = settings;
        }

        public async Task Handle(AppInitMessage message)
        {
            _settings.ApiGatewayUrl = message.GatewayUrl;
        }
    }
}
