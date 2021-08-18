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

        public virtual DateTime CreationDateTime { get; set; }
        public virtual DateTime? RestartDateTime { get; set; }
    }

    public class ChatMapping : EntityMap<Chat>
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
