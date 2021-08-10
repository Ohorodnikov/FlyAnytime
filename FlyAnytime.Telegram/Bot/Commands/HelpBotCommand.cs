﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class HelpBotCommand : BaseBotCommand
    {
        public HelpBotCommand() : base("/help")
        {

        }
        public override async Task<Message> ExecuteAsync(ITelegramBotClient bot, Message message)
        {
            return await bot.SendTextMessageAsync(message.Chat.Id, "Help");
        }
    }
}