using System;
using System.Linq;
using System.Collections.Generic;
using FlyAnytime.Tools;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public interface IConversationStepIniter
    {
        IConversationStepIniter SetNextStep(IConversationStep step);
        IConversationStep GetNextStep(IConversationStep currentStep);
        IConversationStep GetNextStep(Guid currentStepId);
        IConversationStep GetStepById(Guid stepId);
        IConversationStep GetFirstStep();

        void SetChatId(long chatId);
    }

    public class ConversationStepIniter : IConversationStepIniter
    {
        IBotHelper _botHelper;
        long _chatId;
        public ConversationStepIniter(IBotHelper botHelper)
        {
            _botHelper = botHelper;
        }
        private readonly LinkedList<IConversationStep> conversationSteps = new LinkedList<IConversationStep>();

        public void SetChatId(long chatId)
        {
            _chatId = chatId;
        }

        public IConversationStep GetFirstStep()
        {
            var step = conversationSteps.First.Value;
            return InitAndReturnStep(step);
        }

        public IConversationStep GetNextStep(IConversationStep currentStep)
        {
            var step = conversationSteps.Find(currentStep).Next?.Value;

            return InitAndReturnStep(step);
        }

        public IConversationStep GetNextStep(Guid currentStepId)
        {
            var step = conversationSteps
                .EnumerateNodes()
                .FirstOrDefault(x => x.Previous.Value.StepId == currentStepId)
                ?.Value;

            return InitAndReturnStep(step);
        }

        private IConversationStep InitAndReturnStep(IConversationStep step)
        {
            if (_chatId == 0)
                throw new Exception("Call SetChatId() before this method");

            step ??= new EndConversationStep();

            step.SetBotHelperAndChatId(_botHelper, _chatId);

            return step;
        }

        public IConversationStep GetStepById(Guid stepId)
        {
            var step = conversationSteps
                .EnumerateNodes()
                .FirstOrDefault(x => x.Value.StepId == stepId)
                ?.Value;

            return InitAndReturnStep(step);
        }

        public IConversationStepIniter SetNextStep(IConversationStep step)
        {
            conversationSteps.AddLast(step);
            return this;
        }
    }
}
