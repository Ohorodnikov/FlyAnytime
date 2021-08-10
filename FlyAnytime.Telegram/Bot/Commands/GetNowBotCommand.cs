using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class GetNowBotCommand : BaseBotCommand
    {
        public GetNowBotCommand() : base("/getnow") { }

        public override Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message)
        {
            throw new NotImplementedException();
        }
    }
}
