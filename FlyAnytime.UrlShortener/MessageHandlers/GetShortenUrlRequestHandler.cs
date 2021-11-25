using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.UrlShortener;
using FlyAnytime.UrlShortener.Shortener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.MessageHandlers
{
    public class GetShortenUrlRequestHandler : IMessageHandler<GetShortenUrlRequest, GetShortenUrlResponse>
    {
        private readonly IShortener _shortener;

        public GetShortenUrlRequestHandler(IShortener shortener)
        {
            _shortener = shortener;
        }

        public async Task<GetShortenUrlResponse> Handle(GetShortenUrlRequest message)
        {
            var shortUrl = await _shortener.GetShortUrl(message.LongUrl);

            return new GetShortenUrlResponse(shortUrl);
        }
    }
}
