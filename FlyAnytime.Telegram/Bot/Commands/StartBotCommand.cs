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
        IBotHelper _helper;
        public StartBotCommand(IBotHelper helper) : base("/start") 
        {
            _helper = helper;
        }

        public override async Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message)
        {
            if (message.Chat.Type == ChatType.Private)
            {
                await _helper.OnStartPrivateChat(message.Chat.Id);
            }

            if (message.Chat.Type != ChatType.Private)
            {
                var chatId = message.Chat.Id;
                var admins = await bot.GetChatAdministratorsAsync(chatId);
                var creator = admins.FirstOrDefault(x => x.Status == ChatMemberStatus.Creator);
            }

            return await bot.SendTextMessageAsync(message.Chat.Id, "Registered");
        }
    }
}
