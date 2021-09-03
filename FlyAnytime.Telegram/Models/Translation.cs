using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class Translation : Entity<long>
    {
        public virtual long LanguageId { get; set; }
        public virtual Language Language { get; set; }
        public virtual string ItemId { get; set; }
        public virtual string EntityDescriptor { get; set; }

        public virtual string Translate { get; set; }
    }

    public class TranslationMap : EntityMap<Translation, long>
    {
        public TranslationMap() : base("Translation")
        {

        }
        public override void SetMapping(EntityTypeBuilder<Translation> mapBuilder)
        {
            mapBuilder.Ignore(x => x.Id);
            mapBuilder.HasKey(x => new { x.LanguageId, x.ItemId, x.EntityDescriptor });
        }
    }
}
