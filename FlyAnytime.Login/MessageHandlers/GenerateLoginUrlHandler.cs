using FlyAnytime.Messaging.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.MessageHandlers
{
    public class GenerateLoginUrlHandler
    {
        public static void Subscribe()
        {
            var descr = new GetLoginLinkChannel();

            Messaging.Subscribe.SubscribeReturnResult(descr, OnMessage);
        }

        private static LoginLinkResult OnMessage(GetLoginLinkChannelData data)
        {
            var uId = data.UserId;

            return new LoginLinkResult
            {
                Url = "",
            };
        }
    }
}
