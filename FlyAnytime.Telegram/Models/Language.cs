using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class Language : Entity<long>
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Culture { get; set; }
    }

    public class LanguageMapping : EntityMap<Language, long>
    {
        public LanguageMapping() : base("Language")
        {

        }

        public override void SetMapping(EntityTypeBuilder<Language> mapBuilder)
        {
            mapBuilder.HasIndex(x => x.Code).IsUnique();
            mapBuilder.Property(x => x.Name).IsRequired();
            mapBuilder.Property(x => x.Culture).IsRequired();
        }
    }
}
