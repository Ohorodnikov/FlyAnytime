using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class AddOrUpdateBaseSearchSettingsMessage : BaseMessage
    {
        public AddOrUpdateBaseSearchSettingsMessage(
            long chatId, 
            double priceMax, 
            string countryFromFlyCode,
            string cityFromFlyCode,
            string countryToFlyCode)
        {
            ChatId = chatId;
            PriceMax = priceMax;
            CountryFromFlyCode = countryFromFlyCode;
            CityToFlyCode = cityFromFlyCode;
            CountryToFlyCode = countryToFlyCode;
        }

        public long ChatId { get; private set; }
        public double PriceMax { get; private set; }
        public string CountryFromFlyCode { get; private set; }
        public string CityToFlyCode { get; private set; }
        public string CountryToFlyCode { get; private set; }
    }
}
