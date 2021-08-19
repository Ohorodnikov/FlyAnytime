using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class GetOneClickLoginLinkBotCommand : BaseBotCommand
    {
        public GetOneClickLoginLinkBotCommand(IBotHelper helper) : base("/login", helper)
        {

        }

        public override async Task<Message> ExecuteAsync(Message message)
        {
            var userId = message.Chat.Id;

            var channelData = new GetLoginLinkChannelData
            {
                UserId = userId
            };

            var descriptor = new GetLoginLinkChannel
            {
                Data = channelData
            };

            var loginUrl = await Publish.FireAndGetResult(descriptor);

            return await Bot.SendTextMessageAsync(message.Chat.Id, loginUrl.Url);
        }
    }
}
