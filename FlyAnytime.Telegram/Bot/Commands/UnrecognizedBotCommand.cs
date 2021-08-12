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
        public UnrecognizedBotCommand(IBotHelper bot) : base("/unknown", bot) { }

        public override bool CanBeExecuted(string textCommand) => true;

        public override async Task<Message> ExecuteAsync(Message message)
        {
            return await Bot.SendTextMessageAsync(message.Chat.Id, $"{message.Text} is not recognized");
        }
    }
}
