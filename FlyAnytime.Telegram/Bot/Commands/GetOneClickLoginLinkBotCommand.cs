using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Channels;
using FlyAnytime.Messaging.Messages;
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

            var m = new GetLoginLinkRequestMessage(userId);

            var loginUrl = await MessageBus.Publish<GetLoginLinkRequestMessage, GetLoginLinkResponseMessage>(m);

            return await Bot.SendTextMessageAsync(message.Chat.Id, loginUrl.LoginUrl ?? loginUrl.ErrorMessage);
        }
    }
}
