using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{   
    public class ChatSettings : Entity<long>
    {
        public virtual Chat Chat { get; set; }
        public virtual Language UserLanguage { get; set; }
        public virtual ChatSearchSettingsBase SearchSettings { get; set; }
    }

    public class ChatSettingsMapping : EntityMap<ChatSettings, long>
    {
        public ChatSettingsMapping() : base("ChatSettings")
        {

        }

        public override void SetMapping(EntityTypeBuilder<ChatSettings> mapBuilder)
        {
            
        }
    }
}
