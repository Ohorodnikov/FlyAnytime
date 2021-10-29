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

        public override Task<Message> ExecuteAsync(Message message)
        {
            return new UpdatePriceDectinationConversation(BotHelper).Start(message.Chat.Id);
        }
    }
}
