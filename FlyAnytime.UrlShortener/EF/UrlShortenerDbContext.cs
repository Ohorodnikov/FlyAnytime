using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.EF
{
    public class UrlShortenerDbContext : BaseEfContext<UrlShortenerDbContext>
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}
