using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.Shortener
{
    public interface IShortener
    {
        Task<string> GetShortUrl(string longUrl);
        Task<string> GetLongUrl(string shortUrl);
    }
}
