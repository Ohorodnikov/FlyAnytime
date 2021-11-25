using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.Models
{
    public class LongUrl : Entity<long>
    {
        public string OriginalUrl { get; set; }
    }

    public class LongUrlMapping : EntityMap<LongUrl, long>
    {
        public LongUrlMapping() : base("Urls") { }

        public override void SetMapping(EntityTypeBuilder<LongUrl> mapBuilder)
        {
            mapBuilder
                        .Property(x => x.OriginalUrl)
                        .IsRequired(true);
        }
    }
}
