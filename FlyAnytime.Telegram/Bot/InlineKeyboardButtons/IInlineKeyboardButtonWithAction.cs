using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.InlineKeyboardButtons
{
    public interface IInlineKeyboardButtonWithAction
    {
        Guid ButtonId { get; }
        Task OnButtonPress(IBotHelper bot, Message message);
        bool IsPressed(string buttonValue);
    }

    public abstract class InlineKeyboardButtonWithActionBase : InlineKeyboardButton, IInlineKeyboardButtonWithAction
    {
        public InlineKeyboardButtonWithActionBase(string text, string buttonGuid)
        {
            Text = text;
            CallbackData = buttonGuid;
            ButtonId = new Guid(buttonGuid);
        }

        public Guid ButtonId { get; }

        public bool IsPressed(string buttonValue)
        {
            return buttonValue.Equals(CallbackData, StringComparison.InvariantCultureIgnoreCase);
        }

        public abstract Task OnButtonPress(IBotHelper bot, Message message);
    }
}
