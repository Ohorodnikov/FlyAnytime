using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class Poll : Entity<string>
    {
        public virtual Chat Chat { get; set; }
        public virtual bool IsClosed { get; set; }
        public virtual int MessageId { get; set; }

        public virtual IEnumerable<PollItem> Items { get; set; }
    }

    public class PollItem : Entity<long>
    {
        public virtual int Order { get; set; }
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }
    }

    public class PollMapping : EntityMap<Poll, string>
    {
        public PollMapping() : base("Poll")
        {
        }

        public override void SetMapping(EntityTypeBuilder<Poll> mapBuilder)
        {
            mapBuilder
                        .Property(x => x.Id)
                        .IsRequired(true)
                        .ValueGeneratedNever();
        }
    }

    public class PollItemMapping : EntityMap<PollItem, long>
    {
        public PollItemMapping() : base("PollItem")
        {
        }

        public override void SetMapping(EntityTypeBuilder<PollItem> mapBuilder)
        {
        }
    }
}
