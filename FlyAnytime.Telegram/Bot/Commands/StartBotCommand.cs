using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class StartBotCommand : BaseBotCommand
    {
        public StartBotCommand(IBotHelper helper) : base("/start", helper) 
        {
        }

        public override async Task<Message> ExecuteAsync(Message message)
        {
            if (message.Chat.Type == ChatType.Private)
            {
               return await BotHelper.OnStartPrivateChat(message.Chat.Id);
            }

            //if (message.Chat.Type != ChatType.Private)
            //{
            //    var chatId = message.Chat.Id;
            //    var admins = await Bot.GetChatAdministratorsAsync(chatId);
            //    var creator = admins.FirstOrDefault(x => x.Status == ChatMemberStatus.Creator);
            //}

            return await Bot.SendTextMessageAsync(message.Chat.Id, "Registered");
        }
    }
}
