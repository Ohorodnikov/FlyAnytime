using FlyAnytime.Telegram.Bot.Conversations;
using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class UpdateFullChatSettingsCommand : BaseBotCommand
    {
        public UpdateFullChatSettingsCommand(IBotHelper bot) : base("/updateFullSettings", bot)
        {

        }

        public override Task<Message> ExecuteAsync(Message message)
        {
            return new UpdateSettingsFullConversation(BotHelper).Start(message.Chat.Id);
        }
    }
}
