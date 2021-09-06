using FlyAnytime.Telegram.Bot.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class UpdateChatSettingsCommand : BaseBotCommand
    {
        public UpdateChatSettingsCommand(IBotHelper bot) : base("/updateSettings", bot)
        {

        }

        public override Task<Message> ExecuteAsync(Message message)
        {
            return new UpdateSettingsFullConversation(BotHelper).Start(message.Chat.Id);
        }
    }
}
