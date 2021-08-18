using FlyAnytime.Messaging;
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
            var loginUrl = await Publish.FireAndGetResult("getLoginUrl", message.Chat.Id.ToString());

            return await Bot.SendTextMessageAsync(message.Chat.Id, loginUrl);
        }
    }
}
