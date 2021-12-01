using FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Commands
{
    public class UpdateChatSearchSettingsCommand : BaseBotCommand
    {
        public UpdateChatSearchSettingsCommand(IBotHelper bot) : base("/updateSearchParams", bot)
        {

        }

        public override async Task<Message> ExecuteAsync(Message message)
        {
            return await new UpdatePriceDectinationConversation(BotHelper).Start(message.Chat.Id);
        }
    }
}
