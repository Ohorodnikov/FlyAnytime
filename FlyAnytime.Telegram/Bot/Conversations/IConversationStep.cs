using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public interface IConversationStep
    {
        Guid StepId { get; }
        bool WaitAnswer { get; }
        Task<Message> SendConversationBotMessage(IBotHelper bot, long chatId);
        Task OnGetUserAnswer(IBotHelper bot, long chatId, object response);
    }

    public abstract class BaseConversationStep : IConversationStep, IEquatable<BaseConversationStep>
    {
        public abstract Guid StepId { get; }
        public abstract bool WaitAnswer { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseConversationStep);
        }

        public bool Equals(BaseConversationStep other)
        {
            return other != null &&
                   StepId.Equals(other.StepId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StepId);
        }

        public abstract Task OnGetUserAnswer(IBotHelper bot, long chatId, object response);
        public abstract Task<Message> SendConversationBotMessage(IBotHelper bot, long chatId);


    }
}
