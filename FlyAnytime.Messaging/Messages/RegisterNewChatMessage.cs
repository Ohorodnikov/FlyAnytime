using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages
{
    public class RegisterNewChatMessage : BaseMessage
    {
        public RegisterNewChatMessage(
            long userId, 
            string firstName, 
            string lastName, 
            string userName,
            
            long chatId,
            string chatName,
            bool isGroup)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;

            ChatId = chatId;
            ChatName = chatName;
            IsGroup = isGroup;
        }

        public long UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserName { get; private set; }

        public long ChatId { get; private set; }
        public string ChatName { get; private set; }
        public bool IsGroup { get; private set; }
    }
}
