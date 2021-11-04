using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Models.DbModels
{
    public class City : Entity<long>
    {
        public string Code { get; set; }
    }

    public class CityMapping : EntityMap<City, long>
    {
        public CityMapping() : base("City") { }

        public override void SetMapping(EntityTypeBuilder<City> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
        }
    }
}
