using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Channels;
using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class GetOneClickLoginLinkBotCommand : BaseBotCommand
    {
        public GetOneClickLoginLinkBotCommand(IBotHelper helper) : base("/login", helper)
        {

        }

        public override async Task<Message> ExecuteAsync(Message message)
        {
            var userId = message.Chat.Id;

            var m = new GetLoginLinkRequestMessage(userId);

            var loginUrl = await MessageBus.Publish<GetLoginLinkRequestMessage, GetLoginLinkResponseMessage>(m);

            var loginU = new LoginUrl
            {
                Url = loginUrl.LoginUrl
            };

            var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithUrl("Open",
                        //"https://localhost:44320/g/11")
                        loginUrl.LoginUrl)
                    });

            //var res = await Bot.SendTextMessageAsync(chatId: message.Chat.Id,
            //                                      text: "Press button to open",
            //                                      replyMarkup: inlineKeyboard);

            //return res;

            return await Bot.SendTextMessageAsync(message.Chat.Id, $"[Click here to open]({loginUrl.LoginUrl})");// loginUrl.LoginUrl ?? loginUrl.ErrorMessage);
        }
    }
}
