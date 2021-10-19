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
    }

    public class LanguageMapping : EntityMap<Language, long>
    {
        public LanguageMapping() : base("Language")
        {

        }

        public override void SetMapping(EntityTypeBuilder<Language> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
            mapBuilder.Property(x => x.Name).IsRequired();

            //mapBuilder.HasData(GetDefaultValues());
        }

        private IEnumerable<Language> GetDefaultValues()
        {
            return new List<Language>()
            {
                new Language { Id = 1, Name = "English", Code = "En" },
                new Language { Id = 2, Name = "Русский", Code = "Ru" },
                new Language { Id = 3, Name = "Українська", Code = "Ua" },
            };
        }
    }
}
