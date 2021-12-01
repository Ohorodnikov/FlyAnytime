using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps
{
    public class ChangeCurrencyStep : BaseInlineQueryConversationStep
    {
        public override Guid StepId => new Guid("CA6239C6-5704-46B5-95BC-B0A9A97A5D53");

        public override bool WaitAnswer => true;

        protected override async Task<List<OneItemInlineQuery>> GetAnswersForInlineQuery(InlineQuery inlQ)
        {
            var languages = await Bot.DbContext.Set<Currency>()
                .Where(x => x.Code.StartsWith(inlQ.Query))
                .ToListAsync()
                ;

            return
                languages
                .Select(x => new OneItemInlineQuery(x.Id.ToString(), x.Code, x.Code))
                .ToList();
        }

        protected override string GetExplanationText(Models.Chat chat)
        {
            return "Press the button and start writing currency code";
        }

        protected override async Task OnSelectInlineQuery(Message answer)
        {
            var curr = await Bot.DbContext.Set<Currency>()
                .Where(x => x.Code == answer.Text)
                .FirstAsync();

            var msg = new ChangeChatCurrencyMessage(curr.Code, answer.Chat.Id);
            Bot.MessageBus.Publish(msg);
        }
    }
}
