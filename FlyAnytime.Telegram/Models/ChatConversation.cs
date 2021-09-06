using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class ChatConversation : Entity<long>
    {
        public virtual Chat Chat { get; set; }
        public virtual Guid ConversationId { get; set; }
        public virtual Guid ConversationStepId { get; set; }
        public virtual DateTime CreationDateTime { get; set; } = DateTime.UtcNow;
        public virtual bool WaitAnswer { get; set; } = true;
    }

    public class ChatConversationMapping : EntityMap<ChatConversation, long>
    {
        public ChatConversationMapping() : base("Conversation")
        {
        }

        public override void SetMapping(EntityTypeBuilder<ChatConversation> mapBuilder)
        {
            //mapBuilder.Property(x => x.Chat).IsRequired();
            mapBuilder.Property(x => x.ConversationId).IsRequired();
            mapBuilder.Property(x => x.ConversationStepId).IsRequired();
        }
    }
}
