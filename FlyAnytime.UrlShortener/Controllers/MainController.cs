using FlyAnytime.UrlShortener.Shortener;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.Controllers
{
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IShortener _shortener;

        public MainController(IShortener shortener)
        {
            _shortener = shortener;
        }

        [HttpGet("/{shortAlias}")]
        public async Task<IActionResult> RedirectFromShort(string shortAlias)
        {
            var fullLink = await _shortener.GetLongUrl(shortAlias);

            if (fullLink == null)
                return NotFound($"Not found link by alias '{shortAlias}'");

            return Redirect(fullLink);
        }

        [HttpGet("/makeshort")]
        public async Task<IActionResult> ShortenUrl(string longUrl)
        {
            var shortUrl = await _shortener.GetShortUrl(longUrl);

            return new JsonResult(shortUrl);
        }
    }
}
