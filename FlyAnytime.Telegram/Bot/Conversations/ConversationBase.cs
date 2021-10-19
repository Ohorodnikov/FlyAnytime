using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Chat = FlyAnytime.Telegram.Models.Chat;

namespace FlyAnytime.Telegram.Bot.Conversations
{
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
            _stepIniter = new ConversationStepIniter(bot);
            DoStepInit();
        }

        private readonly IConversationStepIniter _stepIniter;

        public IBotHelper Bot { get; }
        public Guid ConversationId { get; }
        public abstract void InitConversationSteps(IConversationStepIniter stepIniter);

        private void DoStepInit()
        {
            InitConversationSteps(_stepIniter);
        }

        public virtual async Task<Message> Start(long chatId)
        {
            _stepIniter.SetChatId(chatId);
            var firstStep = _stepIniter.GetFirstStep();

            return await SaveAndSentStep(chatId, firstStep);
        }

        public virtual async Task<Message> ProcessUserAnswer(long chatId, object response)
        {
            var previousConvStepInDb = Bot.DbContext.Set<ChatConversation>()
                .Where(x => x.Chat.Id == chatId && x.ConversationId == ConversationId)
                .OrderByDescending(x => x.CreationDateTime).First();

            _stepIniter.SetChatId(chatId);

            var prevStep = _stepIniter.GetStepById(previousConvStepInDb.ConversationStepId);

            await prevStep.OnGetUserAnswer(response);
            if (!prevStep.MoveToNextStep)
            {
                return null;
            }

            var nextStep = _stepIniter.GetNextStep(prevStep);

            return await SaveAndSentStep(chatId, nextStep);

        }

        private async Task<Message> SaveAndSentStep(long chatId, IConversationStep step)
        {
            var conv = new ChatConversation
            {
                Chat = await Bot.DbContext.Set<Chat>().FindAsync(chatId),
                ConversationId = ConversationId,
                ConversationStepId = step.StepId,
                WaitAnswer = step.WaitAnswer
            };

            var msg = await step.SendConversationBotMessage();

            Bot.DbContext.Add(conv);

            await Bot.DbContext.SaveChangesAsync();

            return msg;
        }
    }
}
