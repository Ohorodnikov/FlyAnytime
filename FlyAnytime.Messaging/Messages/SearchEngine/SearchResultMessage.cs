using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchEngine
{
    public class OneSearchResult
    {
        public OneSearchResult(string cityFrom,
                               string cityTo,
                               long dateTimeFrom,
                               long dateTimeBack,
                               decimal price,
                               decimal discountPercent,
                               string resultUrl)
        {
            CityFrom = cityFrom;
            CityTo = cityTo;
            DateTimeFrom = dateTimeFrom;
            DateTimeBack = dateTimeBack;
            Price = price;
            DiscountPercent = discountPercent;
            ResultUrl = resultUrl;
        }

        public string CityFrom { get; }
        public string CityTo { get; }

        public long DateTimeFrom { get; }
        public long DateTimeBack { get; }

        public decimal Price { get; }

        /// <summary>
        /// if -0 => discount, if +0 => more expencive than regularPrice
        /// </summary>
        public decimal DiscountPercent { get; }

        public string ResultUrl { get; }
    }

    public class SearchResultMessage : BaseMessage
    {
        public SearchResultMessage(long chatId, List<OneSearchResult> results)
        {
            ChatId = chatId;
            Results = results;
        }

        public long ChatId { get; }

        public List<OneSearchResult> Results { get; }
    }
}
