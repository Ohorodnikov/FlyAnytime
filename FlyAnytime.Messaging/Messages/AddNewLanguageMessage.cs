using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages
{
    public class AddNewLanguageMessage : BaseMessage
    {
        public AddNewLanguageMessage(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; private set; }
        public string Name { get; private set; }
    }
}
