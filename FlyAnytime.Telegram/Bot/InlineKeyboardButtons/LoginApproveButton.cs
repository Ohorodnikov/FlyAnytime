using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.InlineKeyboardButtons
{
    public class LoginApproveButton : InlineKeyboardButtonWithActionBase
    {
        public LoginApproveButton() : base("Approve", "840FCB24-ADD2-4191-9B7D-4A000D3CBC33") { }

        public override async Task OnButtonPress(IBotHelper bot, Message message)
        {
            await InlineKeyboardButtonHelper.HideKeyboard(bot.Bot, message.Chat.Id, message.MessageId);
            //throw new NotImplementedException();
        }
    }
}
