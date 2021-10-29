using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps
{
    public class UpdateCityStep : SearchByLocalizationConversationStep<City>
    {
        public override Guid StepId => new Guid("057F1851-9210-4344-A427-10402D6D0313");

        protected override OneItemInlineQuery ConvertToOneItemInlineQuery(City entity, LocalizationItem localization)
        {
            var displayValue = localization?.Localization ?? entity.Name;
            return new OneItemInlineQuery(entity.Code, displayValue, $"{displayValue}({entity.Code})", entity.Code);
        }

        protected override IEnumerable<(City entity, LocalizationItem loc)> AdditionalFilterForEntities(IEnumerable<(City entity, LocalizationItem loc)> ents)
        {
            var userCountry = Bot.DbContext.Set<Models.Chat>().Find(ChatId).ChatCountry.Id;

            return
                ents.Where(x => x.entity.Country.Id == userCountry);
        }

        protected override async Task OnSelectInlineQuery(Message answer)
        {
            var parts = answer.Text.Split("(");
            var cityCode = parts[1].TrimEnd(')');

            var settings = await Bot.DbContext.Set<Models.Chat>().FindAsync(ChatId);
            settings.ChatCity = await Bot.DbContext.Set<City>().FirstAsync(x => x.Code == cityCode);

            await Bot.DbContext.SaveChangesAsync();

            var msg = new UpdateChatCityMessage(ChatId, cityCode);

            Bot.MessageBus.Publish(msg);

            await Bot.Bot.SendTextMessageAsync(ChatId, $"City {parts[0]} was setted successfully");
        }

        protected override string GetExplanationText(Models.Chat chat)
        {
            return "Press the button to enter your city";
        }
    }
}
