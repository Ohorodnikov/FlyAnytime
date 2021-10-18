using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class LocalizationItem : Entity<long>
    {
        public LocalizationItem() { }
        public virtual long LanguageId { get; set; }
        public virtual Language Language { get; set; }
        public virtual string ItemId { get; set; }
        public virtual string EntityDescriptor { get; set; }

        public virtual string Localization { get; set; }

        public static LocalizationItem Create(IEntityWithLocalization entity, Language language, string value)
        {
            return new LocalizationItem
            {
                Language = language,
                Localization = value,
                ItemId = entity.Id.ToString(),
                EntityDescriptor = entity.TypeDescriptor
            };
        }
    }

    public class TranslationMap : EntityMap<LocalizationItem, long>
    {
        public TranslationMap() : base("LocalizationItem")
        {

        }
        public override void SetMapping(EntityTypeBuilder<LocalizationItem> mapBuilder)
        {
            mapBuilder.Ignore(x => x.Id);
            mapBuilder.HasKey(x => new { x.LanguageId, x.ItemId, x.EntityDescriptor });
        }
    }
}
