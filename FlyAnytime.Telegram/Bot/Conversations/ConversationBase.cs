using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Chat = FlyAnytime.Telegram.Models.Chat;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public interface IConversationStep
    {
        int Order { get; }
        bool WaitAnswer { get; }
        IConversationStep NextStep { get; }
        Task<Message> SendConversationBotMessage(IBotHelper bot, long chatId);
        Task OnGetUserAnswer(IBotHelper bot, long chatId, object response);
    }

    public class EndConversationStep : IConversationStep
    {
        public int Order => int.MaxValue;

        public bool WaitAnswer => false;

        public IConversationStep NextStep => null;

        public Task OnGetUserAnswer(IBotHelper bot, long chatId, object response)
        {
            throw new NotSupportedException();
        }

        public async Task<Message> SendConversationBotMessage(IBotHelper bot, long chatId)
        {
            return null;
        }
    }

    public interface IConversation
    {
        public Guid ConversationId { get; }
        Task<Message> Start(long chatId);
        Task<Message> ProcessUserAnswer(long chatId, object response);
    }

    public abstract class ConversationBase : IConversation
    {
        protected ConversationBase(IBotHelper bot, Guid conversationId)
        {
            Bot = bot;
            ConversationId = conversationId;
        }

        public IBotHelper Bot { get; }
        public Guid ConversationId { get; }

        public abstract IEnumerable<IConversationStep> GetConversationSteps();
        public virtual async Task<Message> Start(long chatId)
        {
            var firstStep = GetConversationSteps().OrderBy(x => x.Order).First();

            return await SaveAndSentStep(chatId, firstStep);
        }

        public virtual async Task<Message> ProcessUserAnswer(long chatId, object response)
        {
            var previousConvStepInDb = Bot.DbContext.Set<ChatConversation>()
                .Where(x => x.Chat.Id == chatId && x.ConversationId == ConversationId)
                .OrderByDescending(x => x.CreationDateTime).First();

            var prevStep = GetConversationSteps().First(x => x.Order == previousConvStepInDb.ConversationStep);
            await prevStep.OnGetUserAnswer(Bot, chatId, response);
            var nextStep = prevStep.NextStep;

            if (nextStep == null)
            {
                return await SaveAndSentStep(chatId, new EndConversationStep());
            }

            return await SaveAndSentStep(chatId, nextStep);

        }

        private async Task<Message> SaveAndSentStep(long chatId, IConversationStep step)
        {
            var conv = new ChatConversation
            {
                Chat = await Bot.DbContext.Set<Chat>().FindAsync(chatId),
                ConversationId = ConversationId,
                ConversationStep = step.Order,
                WaitAnswer = step.WaitAnswer
            };

            var msg = await step.SendConversationBotMessage(Bot, chatId);

            Bot.DbContext.Add(conv);

            await Bot.DbContext.SaveChangesAsync();

            return msg;
        }
    }
}
