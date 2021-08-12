using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class HelpBotCommand : BaseBotCommand
    {
        public HelpBotCommand(IBotHelper bot) : base("/help", bot) { }

        public override async Task<Message> ExecuteAsync(Message message)
        {
            return await Bot.SendTextMessageAsync(message.Chat.Id, "Help");
        }
    }
}
