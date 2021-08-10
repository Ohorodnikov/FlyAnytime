using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class SubscribeBotCommand : BaseBotCommand
    {
        public SubscribeBotCommand() : base("/subscribe") { }

        public override Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message)
        {
            throw new NotImplementedException();
        }
    }
}
