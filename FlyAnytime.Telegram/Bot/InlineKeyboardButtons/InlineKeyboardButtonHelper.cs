using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.InlineKeyboardButtons
{
    public static class InlineKeyboardButtonHelper
    {
        public static async Task HideKeyboard(ITelegramBotClient bot, ChatId chatId, int messageId)
        {
            await bot.EditMessageReplyMarkupAsync(chatId, messageId, InlineKeyboardMarkup.Empty());
        }
    }
}
