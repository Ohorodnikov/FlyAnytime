using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class AddOrUpdateCountryMessage : BaseMessage
    {
        public AddOrUpdateCountryMessage(string code, string name, Dictionary<string, string> languageCode2value)
        {
            Code = code;
            Name = name;
            LanguageCode2Value = languageCode2value;
        }

        public string Code { get; private set; }
        public string Name { get; private set; }
        public Dictionary<string, string> LanguageCode2Value { get; private set; }
    }
}
