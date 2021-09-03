using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public class UpdateSettingsConversation : ConversationBase
    {
        static readonly Guid _id = new Guid("37F7C2D2-879D-4E3B-9136-E97DA878FFE0");
        public UpdateSettingsConversation(IBotHelper bot) : base(bot, _id)
        {
        }

        public override IEnumerable<IConversationStep> GetConversationSteps()
        {
            return new[]
            {
                new UpdateUserLanguageStep(),
            };
        }
    }

    public class UpdateUserLanguageStep : IConversationStep
    {
        public int Order => 0;
        public IConversationStep NextStep => null;

        public bool WaitAnswer => true;

        public async Task OnGetUserAnswer(IBotHelper bot, long chatId, object response)
        {
            if (!(response is PollAnswer poll))
                return;

            var savedPoll = bot.DbContext.Set<Models.Poll>().Find(poll.PollId);

            var selectedItem = savedPoll.Items.FirstOrDefault(x => x.Order == poll.OptionIds[0]);

            var settings = bot.DbContext.Set<ChatSettings>().First(x => x.Chat.Id == chatId);

            settings.UserLanguage = bot.DbContext.Set<Language>().Find(long.Parse(selectedItem.Value));

            await bot.Bot.StopPollAsync(chatId, savedPoll.MessageId);

            savedPoll.IsClosed = true;

            await bot.DbContext.SaveChangesAsync();
        }

        public async Task<Message> SendConversationBotMessage(IBotHelper bot, long chatId)
        {
            var languages = bot.DbContext.Set<Language>().ToList();

            var pollItems = new List<PollItem>();

            for (int i = 0; i < languages.Count; i++)
            {
                pollItems.Add(new PollItem
                {
                    Order = i,
                    Text = languages[i].Name,
                    Value = languages[i].Id.ToString()
                });
            }

            var poll = new Models.Poll
            {
                Chat = bot.DbContext.Set<Models.Chat>().Find(chatId),
                IsClosed = false,
                Items = pollItems
            };

            var msg = await bot.Bot.SendPollAsync(chatId, "Select your language", pollItems.Select(x => x.Text), false, PollType.Regular, false);

            poll.Id = msg.Poll.Id;
            poll.MessageId = msg.MessageId;

            bot.DbContext.Add(poll);

            await bot.DbContext.SaveChangesAsync();

            return msg;
        }
    }
}
