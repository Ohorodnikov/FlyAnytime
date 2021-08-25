using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages
{
    public class RegisterNewUserMessage : BaseMessage
    {
        public RegisterNewUserMessage(long userId, string firstName, string lastName, string userName)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
        }

        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}
