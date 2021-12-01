using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class ChangeCurrencyCommand : BaseBotCommand
    {
        public ChangeCurrencyCommand(IBotHelper botHelper) : base("/changeCurrency", botHelper)
        {
        }

        public override async Task<Message> ExecuteAsync(Message message)
        {
            return await new UpdateCurrencyConversation(BotHelper).Start(message.Chat.Id);
        }
    }
}
