using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class City : Entity<long>, IEntityWithLocalization
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        public string TypeDescriptor => "City";
        public virtual Country Country { get; set; }
    }

    public class SearchCityMapping : EntityMap<City, long>
    {
        public SearchCityMapping() : base("City") { }

        public override void SetMapping(EntityTypeBuilder<City> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
            mapBuilder.Property(x => x.Name).IsRequired();
        }
    }
}
