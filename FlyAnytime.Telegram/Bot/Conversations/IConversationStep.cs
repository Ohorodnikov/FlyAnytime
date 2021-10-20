using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Bot.Conversations
{
    public interface IConversationStep
    {
        Guid StepId { get; }
        bool WaitAnswer { get; }
        bool MoveToNextStep { get; }
        IBotHelper Bot { get; }
        long ChatId { get; }

        void SetBotHelperAndChatId(IBotHelper bot, long chatId);

        Task<Message> SendConversationBotMessage();
        Task OnGetUserAnswer(int sentMessageId, object response);
    }

    public abstract class BaseConversationStep : IConversationStep, IEquatable<BaseConversationStep>
    {
        public abstract Guid StepId { get; }
        public abstract bool WaitAnswer { get; }

        public bool MoveToNextStep { get; protected set; } = true;
        public IBotHelper Bot { get; private set; }
        public long ChatId { get; private set; }

        public void SetBotHelperAndChatId(IBotHelper bot, long chatId)
        {
            Bot = bot;
            ChatId = chatId;
        }
        

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

        public abstract Task OnGetUserAnswer(int sentMessageId, object response);
        public abstract Task<Message> SendConversationBotMessage();
    }
}
