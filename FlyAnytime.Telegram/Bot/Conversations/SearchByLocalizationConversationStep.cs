using FlyAnytime.Core.Entity;
using FlyAnytime.Telegram.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public abstract class SearchByLocalizationConversationStep<TEntity> : BaseInlineQueryConversationStep
         where TEntity : class, IEntityWithLocalization, new()
    {
        public override bool WaitAnswer => true;

        protected abstract OneItemInlineQuery ConvertToOneItemInlineQuery(TEntity entity, LocalizationItem localization);

        protected virtual IEnumerable<(TEntity entity, LocalizationItem loc)> AdditionalFilterForEntities(IEnumerable<(TEntity entity, LocalizationItem loc)> ents)
        {
            return ents;
        }

        protected override async Task<List<OneItemInlineQuery>> GetAnswersForInlineQuery(InlineQuery inlQ)
        {
            var chat = await Bot.DbContext.Set<Models.Chat>().FindAsync(ChatId);

            var helper = Bot.ServiceProvider.GetService<ILocalizationHelper>();
            var items = await helper.FindEntitiesByLocalization<TEntity>(chat.UserLanguage, inlQ.Query);

            items = AdditionalFilterForEntities(items);

            return
                items.Select(x => ConvertToOneItemInlineQuery(x.entity, x.localization))
                .ToList();
        }
    }
}
