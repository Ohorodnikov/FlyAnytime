using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class AddOrUpdateAirportMessage : BaseMessage
    {
        public AddOrUpdateAirportMessage(string code, string name, string cityCode, Dictionary<string, string> languageCode2value)
        {
            Code = code;
            Name = name;
            CityCode = cityCode;
            LanguageCode2Value = languageCode2value;
        }

        public string Code { get; private set; }
        public string Name { get; private set; }
        public string CityCode { get; private set; }
        public Dictionary<string, string> LanguageCode2Value { get; private set; }
    }
}
