using FlyAnytime.Telegram.EF;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public class User : Entity<long>
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string UserName { get; set; }

        public virtual DateTime CreationDateTime { get; set; }
    }

    public class UserMapping : EntityMap<User>
    {
        public UserMapping() : base("User") { }

        public override void SetMapping(EntityTypeBuilder<User> mapBuilder)
        {
            mapBuilder
                        .Property(x => x.Id)
                        .IsRequired(true)
                        .ValueGeneratedNever();

            mapBuilder.Property(x => x.CreationDateTime).IsRequired(true);
        }
    }
}
