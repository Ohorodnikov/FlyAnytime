using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class ChatSearchSettingsBase : Entity<long>
    {
        public virtual Chat Chat { get; set; }
        public virtual SearchCity ChatCity { get; set; }
        public virtual SearchCountry ChatCountry { get; set; }
    }

    public class ChatSearchSettingsBaseMapping : EntityMap<ChatSearchSettingsBase, long>
    {
        public ChatSearchSettingsBaseMapping() : base("ChatSerSet")
        {

        }

        public override void SetMapping(EntityTypeBuilder<ChatSearchSettingsBase> mapBuilder)
        {
            //throw new NotImplementedException();
        }
    }
}
