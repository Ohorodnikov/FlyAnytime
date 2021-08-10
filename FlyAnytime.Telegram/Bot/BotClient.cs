using FlyAnytime.Telegram.Bot.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlyAnytime.Telegram.Bot
{
    public class BotClient
    {
        private ITelegramBotClient _botClient;
        IServiceProvider _serviceProvider;

        public BotClient(ITelegramBotClient botClient, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _botClient = botClient;
        }

        public async Task ProcessUpdate(Update update)
        {
            Func<Message, Task> handler = null;
            switch (update.Type)
            {
                case UpdateType.Unknown:
                    break;
                case UpdateType.Message:
                    handler = MessageReceive;
                    break;
                case UpdateType.InlineQuery:
                    break;
                case UpdateType.ChosenInlineResult:
                    break;
                case UpdateType.CallbackQuery:
                    break;
                case UpdateType.EditedMessage:
                    break;
                case UpdateType.ChannelPost:
                    break;
                case UpdateType.EditedChannelPost:
                    break;
                case UpdateType.ShippingQuery:
                    break;
                case UpdateType.PreCheckoutQuery:
                    break;
                case UpdateType.Poll:
                    break;
                case UpdateType.PollAnswer:
                    break;
                case UpdateType.MyChatMember:
                    break;
                case UpdateType.ChatMember:
                    break;
                default:
                    break;
            }

            try
            {
                await handler(update.Message);
            }
            catch (Exception exception)
            {
                //await HandleErrorAsync(exception);
            }
        }

        private async Task MessageReceive(Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            var allCommands = _serviceProvider.GetServices<IBotCommand>();
            var messageCommand = message.Text.Split(" ")[0];
            foreach (var cmd in allCommands)
            {
                if (cmd.CanBeExecuted(messageCommand))
                {
                    await cmd.ExecuteAsync(_botClient, message);
                    return;
                }
            }
        }
    }
}
