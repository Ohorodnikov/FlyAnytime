using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class AddOrUpdateCityMessage : BaseMessage
    {
        public AddOrUpdateCityMessage(string code, string name, string countryCode, Dictionary<string, string> language2value)
        {
            Code = code;
            Name = name;
            CountryCode = countryCode;
            LanguageCode2Value = language2value;
        }

        public string Code { get; private set; }
        public string Name { get; private set; }
        public string CountryCode { get; private set; }
        public Dictionary<string, string> LanguageCode2Value { get; private set; }
    }
}
