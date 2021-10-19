using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.Conversations.UpdateSettingsConversation.Steps
{

    public class UpdateCountryStep : SearchByLocalizationConversationStep<SearchCountry>
    {
        public override Guid StepId => new Guid("E87E072F-076A-45A7-B311-190534D4D791");

        protected override OneItemInlineQuery ConvertToOneItemInlineQuery(SearchCountry entity, LocalizationItem localization)
        {
            return new OneItemInlineQuery(entity.Code, localization.Localization, $"{localization.Localization}({entity.Code})", entity.Code);
        }

        protected override async Task OnSelectInlineQuery(Message answer)
        {
            var parts = answer.Text.Split("(");
            var countryCode = parts[1].TrimEnd(')');

            var settings = await Bot.DbContext.Set<Models.Chat>().FindAsync(ChatId);
            settings.SearchSettings.ChatCountry = await Bot.DbContext.Set<SearchCountry>().FirstAsync(x => x.Code == countryCode);

            await Bot.DbContext.SaveChangesAsync();

            var msg = new UpdateChatCountryMessage(ChatId, countryCode);

            Bot.MessageBus.Publish(msg);

            await Bot.Bot.SendTextMessageAsync(ChatId, $"Country {parts[0]} was setted successfully");
        }

        protected override string GetExplanationText(Language language)
        {
            return "Press the button to enter your country";
        }
    }
}
