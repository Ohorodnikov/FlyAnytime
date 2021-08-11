using FlyAnytime.Telegram.Bot.InlineKeyboardButtons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class Send2ButtonBotCommand : BaseBotCommand
    {
        public Send2ButtonBotCommand() : base("/loginBtn") { }

        public override async Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
                    {
                        new LoginApproveButton(),
                        new LoginDeclineButton(),
                    });

            var res = await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Choose",
                                                  replyMarkup: inlineKeyboard);

            return res;
        }
    }
}
