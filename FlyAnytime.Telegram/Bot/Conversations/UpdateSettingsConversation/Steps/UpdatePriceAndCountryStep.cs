using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps
{
    public class UpdatePriceAndCountryStep : BaseInlineQueryConversationStep
    {
        public override Guid StepId => new Guid("FFC98AE5-D462-4F27-A2D6-B5DB78DAC71B");

        public override bool WaitAnswer => true;

        protected override string GetExplanationText(Language language)
        {
            return "Press and type max price in USD and Country you want to fly to";
        }

        protected override async Task<List<OneItemInlineQuery>> GetAnswersForInlineQuery(InlineQuery inlQ)
        {
            var parts = inlQ.Query.Trim().Split(' ');
            var priceStr = parts[0];
            var countrySearchString = string.Empty;
            if (parts.Length == 2)
                countrySearchString = parts[1];

            if (double.TryParse(priceStr, out _))
            {
                var chat = await Bot.DbContext.Set<Models.Chat>().FindAsync(ChatId);

                var helper = Bot.ServiceProvider.GetService<ILocalizationHelper>();
                var items = await helper.FindEntitiesByLocalization<SearchCountry>(chat.UserLanguage, countrySearchString);

                return
                    items.Select(x => new OneItemInlineQuery(x.entity.Code, x.localization.Localization, $"{priceStr} {x.localization.Localization}({x.entity.Code})", x.entity.Code))
                    .ToList();
            }

            return new List<OneItemInlineQuery>();
        }

        protected override async Task OnSelectInlineQuery(Message answer)
        {
            var parts = answer.Text.Trim().Split(' ');
            var price = parts[0];

            var countryStr = parts[1];
            var countryCode = countryStr.Split("(")[1].Trim(')');

            var message = new AddOrUpdateBaseSearchSettingsMessage(ChatId, double.Parse(price), countryCode);

            Bot.MessageBus.Publish(message);
        }
    }
}
