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
    }

    public class ConversationStepIniter : IConversationStepIniter
    {
        private readonly LinkedList<IConversationStep> conversationSteps = new LinkedList<IConversationStep>();

        public IConversationStep GetFirstStep()
        {
            return conversationSteps.First.Value;
        }

        public IConversationStep GetNextStep(IConversationStep currentStep)
        {
            return conversationSteps.Find(currentStep).Next.Value;
        }

        public IConversationStep GetNextStep(Guid currentStepId)
        {
            return conversationSteps
                .EnumerateNodes()
                .FirstOrDefault(x => x.Previous.Value.StepId == currentStepId)
                ?.Value;
        }

        public IConversationStep GetStepById(Guid stepId)
        {
            return conversationSteps
                .EnumerateNodes()
                .FirstOrDefault(x => x.Value.StepId == stepId)
                ?.Value;
        }

        public IConversationStepIniter SetNextStep(IConversationStep step)
        {
            conversationSteps.AddLast(step);
            return this;
        }
    }
}
