using FlyAnytime.UrlShortener.Controllers;
using FlyAnytime.UrlShortener.EF;
using FlyAnytime.UrlShortener.Helpers;
using FlyAnytime.UrlShortener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.Shortener
{
    public class UrlShortener : IShortener
    {
        private readonly UrlShortenerDbContext _context;
        private readonly ILongShortener _longShortener;
        private readonly LinkGenerator _linkGenerator;
        private readonly IConfiguration _configuration;

        public UrlShortener(UrlShortenerDbContext context, ILongShortener longShortener, LinkGenerator linkGenerator, IConfiguration configuration)
        {
            _context = context;
            _longShortener = longShortener;
            _linkGenerator = linkGenerator;
            _configuration = configuration;
        }

        public async Task<string> GetLongUrl(string shortUrl)
        {
            var id = _longShortener.Restore(shortUrl);

            var data = await _context.Set<LongUrl>().FindAsync(id);

            return data?.OriginalUrl;
        }

        public async Task<string> GetShortUrl(string longUrl)
        {
            var model = new LongUrl
            {
                OriginalUrl = longUrl
            };

            _context.Add(model);
            await _context.SaveChangesAsync();

            var data = new
            {
                shortAlias = _longShortener.Compress(model.Id)
            };
            var hostInfo = _configuration.GetSection("HostInfo").Get<HostInfo>();

            return _linkGenerator.CreateAbsoluteUrl<MainController>(x => x.RedirectFromShort(null), data, hostInfo);
        }
    }
}
