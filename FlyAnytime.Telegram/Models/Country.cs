using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class Country : Entity<long>, IEntityWithLocalization
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        public virtual string CurrencyCode { get; set; }

        public string TypeDescriptor => "Country";
    }

    public class SearchCountryMapping : EntityMap<Country, long>
    {
        public SearchCountryMapping() : base("SearchCountry") { }

        public override void SetMapping(EntityTypeBuilder<Country> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
            mapBuilder.Property(x => x.Name).IsRequired();
            mapBuilder.Property(x => x.CurrencyCode).IsRequired();
        }
    }
}
