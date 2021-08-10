using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public interface IBotCommand
    {
        bool CanBeExecuted(string textCommand);
        Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message);
    }

    public abstract class BaseBotCommand : IBotCommand
    {
        protected BaseBotCommand(string commandName)
        {
            Command = commandName;
        }
        public string Command { get; }

        public abstract Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message);

        public virtual bool CanBeExecuted(string textCommand)
        {
            return Command.Equals(textCommand, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
