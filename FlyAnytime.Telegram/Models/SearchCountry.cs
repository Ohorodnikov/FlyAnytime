using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class SearchCountry : Entity<long>, IEntityWithLocalization
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        public virtual string CurrencyCode { get; set; }

        public string TypeDescriptor => "SearchCountry";
    }

    public class SearchCountryMapping : EntityMap<SearchCountry, long>
    {
        public SearchCountryMapping() : base("SearchCountry") { }

        public override void SetMapping(EntityTypeBuilder<SearchCountry> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
            mapBuilder.Property(x => x.Name).IsRequired();
        }
    }
}
