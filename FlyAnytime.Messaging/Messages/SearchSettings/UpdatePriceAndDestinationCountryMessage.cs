using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class UpdatePriceAndDestinationCountryMessage : BaseMessage
    {
        public UpdatePriceAndDestinationCountryMessage(
            long chatId, 
            decimal priceMax,
            string countryToFlyCode)
        {
            ChatId = chatId;
            PriceMax = priceMax;
            CountryToFlyCode = countryToFlyCode;
        }

        public long ChatId { get; private set; }
        public decimal PriceMax { get; private set; }
        public string CountryToFlyCode { get; private set; }
    }
}
