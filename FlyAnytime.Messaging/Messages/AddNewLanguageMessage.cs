using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages
{
    public class AddNewLanguageMessage : BaseMessage
    {
        public AddNewLanguageMessage(string code, string name, string culture)
        {
            Code = code;
            Name = name;
            Culture = culture;
        }

        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Culture { get; private set; }
    }
}
