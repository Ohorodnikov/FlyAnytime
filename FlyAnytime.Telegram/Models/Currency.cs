using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class Currency : Entity<int>
    {
        public string Code { get; set; }
    }

    public class CurrencyMap : EntityMap<Currency, int>
    {
        public CurrencyMap() : base("Currency") { }

        public override void SetMapping(EntityTypeBuilder<Currency> mapBuilder)
        {
            mapBuilder.HasIndex(x => x.Code).IsUnique();
            mapBuilder.Property(x => x.Code).HasMaxLength(10);
        }
    }
}
