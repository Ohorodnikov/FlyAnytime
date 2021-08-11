using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.InlineKeyboardButtons
{
    public class LoginDeclineButton : InlineKeyboardButtonWithActionBase
    {
        public LoginDeclineButton() : base("Decline", "8150049C-11E5-42BC-AC62-AB6A5F281B0B") { }

        public override async Task OnButtonPress(ITelegramBotClient bot, Message message)
        {
            await InlineKeyboardButtonHelper.HideKeyboard(bot, message.Chat.Id, message.MessageId);
            await bot.EditMessageTextAsync(message.Chat.Id, message.MessageId, "Login declined");
            //throw new NotImplementedException();
        }
    }
}
