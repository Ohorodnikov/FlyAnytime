using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.Telegram.Bot;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class ChangeChatCurrencyHandler : IMessageHandler<ChangeChatCurrencyMessage>
    {
        private readonly TelegramContext _dbContext;
        private readonly ITelegramBotClient _bot;

        public ChangeChatCurrencyHandler(IBotHelper botHelper)
        {
            _dbContext = botHelper.DbContext;
            _bot = botHelper.Bot;
        }

        public async Task Handle(ChangeChatCurrencyMessage message)
        {
            var curr = await _dbContext.Set<Currency>().Where(x => x.Code == message.CurrencyCode).FirstOrDefaultAsync();

            if (curr != null)
            {
                var chat = await _dbContext.Set<Chat>().FindAsync(message.ChatId);

                if (chat != null)
                {
                    chat.Currency = curr;

                    await _dbContext.SaveChangesAsync();

                    await _bot.SendTextMessageAsync(message.ChatId, $"Chat currency has been changed on {curr.Code}");
                }
            }
        }
    }
}
