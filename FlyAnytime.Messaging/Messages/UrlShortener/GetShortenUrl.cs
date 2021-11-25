using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.UrlShortener
{
    public class GetShortenUrlRequest : BaseMessage
    {
        public GetShortenUrlRequest(string longUrl)
        {
            LongUrl = longUrl;
        }

        public string LongUrl { get; }
    }

    public class GetShortenUrlResponse : BaseResponseMessage<GetShortenUrlRequest>
    {
        private GetShortenUrlResponse() { }

        public GetShortenUrlResponse(string shortUrl)
        {
            ShortUrl = shortUrl;
        }

        public string ShortUrl { get; }
    }
}
