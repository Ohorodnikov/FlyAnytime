using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class AllMyChatsBotCommand : BaseBotCommand
    {
        public AllMyChatsBotCommand(IBotHelper bot) : base("/allmycahts", bot)
        {
        }

        public override Task<Message> ExecuteAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
