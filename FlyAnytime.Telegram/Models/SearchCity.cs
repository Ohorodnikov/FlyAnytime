using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class SearchCity : Entity<long>
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        public virtual SearchCountry Country { get; set; }
    }

    public class SearchCityMapping : EntityMap<SearchCity, long>
    {
        public SearchCityMapping() : base("SearchCity") { }

        public override void SetMapping(EntityTypeBuilder<SearchCity> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
            mapBuilder.Property(x => x.Name).IsRequired();
        }
    }
}
