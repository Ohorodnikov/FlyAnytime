using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class UnrecognizedBotCommand : BaseBotCommand
    {
        public UnrecognizedBotCommand() : base("/unknown")
        {
        }

        public override bool CanBeExecuted(string textCommand)
        {
            return true;
        }

        public override async Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message)
        {
            return await bot.SendTextMessageAsync(message.Chat.Id, $"{message.Text} is not recognized");
        }
    }
}
