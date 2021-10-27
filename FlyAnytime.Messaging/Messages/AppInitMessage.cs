using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages
{
    public class AppInitMessage : BaseMessage
    {
        public AppInitMessage(string gatewayUrl)
        {
            GatewayUrl = gatewayUrl;
        }

        public string GatewayUrl { get; }
    }
}
