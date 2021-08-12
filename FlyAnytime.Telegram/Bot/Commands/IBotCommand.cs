using FlyAnytime.Telegram.EF;
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
        Task<Message> ExecuteAsync(Message message);
    }

    public abstract class BaseBotCommand : IBotCommand
    {
        protected BaseBotCommand(string commandName, IBotHelper botHelper)
        {
            if (!commandName.StartsWith("/"))
                throw new ArgumentException("Command must starts from '/'", nameof(commandName));

            Command = commandName;
            BotHelper = botHelper;
        }

        protected IBotHelper BotHelper { get; }
        protected ITelegramBotClient Bot => BotHelper.Bot;
        protected TelegramContext DbContext => BotHelper.DbContext;


        public string Command { get; }

        public abstract Task<Message> ExecuteAsync(Message message);

        public virtual bool CanBeExecuted(string textCommand)
        {
            return Command.Equals(textCommand, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
