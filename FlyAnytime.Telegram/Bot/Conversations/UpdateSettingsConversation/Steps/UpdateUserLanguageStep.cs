using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps
{
    public class UpdateUserLanguageStep : BaseInlineQueryConversationStep
    {
        public override Guid StepId => new Guid("E7BE577C-3E25-4772-823F-2012CDF43960");

        public override bool WaitAnswer => true;

        protected override string GetExplanationText(Language language)
        {
            return "Press the button to select your language";
        }

        protected override async Task<List<OneItemInlineQuery>> GetAnswersForInlineQuery(InlineQuery inlQ)
        {
            var languages = await Bot.DbContext.Set<Language>()
                .Where(x => x.Name.StartsWith(inlQ.Query))
                .ToListAsync()
                ;

            return
                languages
                .Select(x => new OneItemInlineQuery(x.Id.ToString(), x.Name, x.Name))
                .ToList();
        }

        protected override async Task OnSelectInlineQuery(Message answer)
        {
            var language = await Bot.DbContext.Set<Language>()
                .Where(x => x.Name == answer.Text)
                .FirstAsync();

            var settings = await Bot.DbContext.Set<Models.Chat>().FindAsync(ChatId);

            settings.UserLanguage = language;

            await Bot.DbContext.SaveChangesAsync();

            var message = new UpdateChatLanguageMessage(ChatId, language.Code);

            Bot.MessageBus.Publish(message);
        }
    }
}
