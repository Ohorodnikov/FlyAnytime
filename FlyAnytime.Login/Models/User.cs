using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public class User : Entity<long>
    {
    }

    public class UserMapping : EntityMap<User, long>
    {
        public UserMapping() : base("User") { }

        public override void SetMapping(EntityTypeBuilder<User> mapBuilder)
        {
            mapBuilder
                        .Property(x => x.Id)
                        .IsRequired(true)
                        .ValueGeneratedNever();
        }
    }
}
