using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class AddOrUpdateCountryMessage : BaseMessage
    {
        public AddOrUpdateCountryMessage(string code, string name, string currencyToSearch, Dictionary<string, string> languageCode2value)
        {
            Code = code;
            Name = name;

            LanguageCode2Value = languageCode2value;
            CurrencyToSearch = currencyToSearch;
        }

        public string Code { get; private set; }
        public string Name { get; private set; }
        public string CurrencyToSearch { get; private set; }
        public Dictionary<string, string> LanguageCode2Value { get; private set; }
    }
}
