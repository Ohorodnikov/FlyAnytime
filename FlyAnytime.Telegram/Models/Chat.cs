using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using FlyAnytime.Telegram.EF;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FlyAnytime.Telegram.Models
{
    public class Chat : Entity<long>
    {
        public virtual User ChatOwner { get; set; }
        public virtual bool IsGroup { get; set; }
        public virtual string Title { get; set; }
        public virtual string Username { get; set; }
        public virtual bool HasAdminRights { get; set; }
        public virtual bool IsPaused { get; set; }
        public virtual bool IsRemovedFromChat { get; set; }

        public virtual long CreationDateTime { get; set; }
        public virtual long? RestartDateTime { get; set; }

        public virtual Language UserLanguage { get; set; }
        public virtual City ChatCity { get; set; }
        public virtual Country ChatCountry { get; set; }
        public virtual Currency Currency { get; set; }
    }

    public class ChatMapping : EntityMap<Chat, long>
    {
        public ChatMapping() : base("Chat") { }

        public override void SetMapping(EntityTypeBuilder<Chat> mapBuilder)
        {
            mapBuilder
                        .Property(x => x.Id)
                        .IsRequired(true)
                        .ValueGeneratedNever();

            mapBuilder.Property(x => x.CreationDateTime).IsRequired(true);
        }
    }
}
