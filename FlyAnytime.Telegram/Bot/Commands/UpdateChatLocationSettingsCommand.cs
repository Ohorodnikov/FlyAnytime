using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class UpdateChatLocationSettingsCommand : BaseBotCommand
    {
        public UpdateChatLocationSettingsCommand(IBotHelper bot) : base("/updateMyLocation", bot)
        {

        }

        public override async Task<Message> ExecuteAsync(Message message)
        {
            return await new UpdateSettingsCountryCityConversation(BotHelper).Start(message.Chat.Id);
        }
    }
}
