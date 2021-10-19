using FlyAnytime.Telegram.Models;
using FlyAnytime.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public class OneItemInlineQuery
    {
        public OneItemInlineQuery(string id, string dropDownValue, string valueAfterSelect, string dropDownDescription = null)
        {
            Id = id;
            DropDownValue = dropDownValue;
            ValueAfterSelect = valueAfterSelect;
            DropDownDescription = dropDownDescription;
        }

        public string Id { get; set; }
        public string DropDownValue { get; set; }
        public string DropDownDescription { get; set; }
        public string ValueAfterSelect { get; set; }
    }

    public abstract class BaseInlineQueryConversationStep : BaseConversationStep
    {
        public override async Task OnGetUserAnswer(object response)
        {
            if (response is InlineQuery inlQ)
            {
                MoveToNextStep = false;
                var possibleAnswers = await GetAnswersForInlineQuery(inlQ);

                var answersList = new List<InlineQueryResultBase>(40);

                for (int i = 0; i < Math.Min(40, possibleAnswers.Count); i++)
                {
                    var item = possibleAnswers[i];
                    var textMessageAfterSelect = new InputTextMessageContent(item.ValueAfterSelect);

                    var r = new InlineQueryResultArticle(item.Id, item.DropDownValue, textMessageAfterSelect)
                    {
                        Description = item.DropDownDescription
                    };

                    answersList.Add(r);
                }

                await SendAnswerInlineQuery(inlQ, answersList);
            }
            else
            {
                await OnSelectInlineQuery((Message)response);
            }
        }

        protected abstract string GetExplanationText(Language language);
        public override async Task<Message> SendConversationBotMessage()
        {
            var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Button");

            var inlineKeyboard = new InlineKeyboardMarkup(button);

            var chat = await Bot.DbContext.Set<Models.Chat>().FindAsync(ChatId);

            var lang = chat.UserLanguage;

            var res = await Bot.Bot.SendTextMessageAsync(ChatId,
                                                  text: GetExplanationText(lang),
                                                  replyMarkup: inlineKeyboard);

            return res;
        }

        protected virtual async Task SendAnswerInlineQuery(InlineQuery inlQ, List<InlineQueryResultBase> possibleAnswers)
        {
            await Bot.Bot.AnswerInlineQueryAsync(inlQ.Id, possibleAnswers, 0, true);
        }

        protected abstract Task<List<OneItemInlineQuery>> GetAnswersForInlineQuery(InlineQuery inlQ);
        protected abstract Task OnSelectInlineQuery(Message answer);
    }
}
