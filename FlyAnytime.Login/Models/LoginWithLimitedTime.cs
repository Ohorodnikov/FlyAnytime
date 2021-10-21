using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public abstract class LoginWithLimitedTime : BaseLogin
    {
        public virtual long CreationDateTime { get; set; }
        public virtual long ExpireDateTime { get; set; }
    }

    public abstract class LoginWithLimitedTimeMapping<TLoginModel> : EntityMap<TLoginModel, long>
        where TLoginModel : LoginWithLimitedTime
    {
        public LoginWithLimitedTimeMapping(string tableName) : base(tableName) { }

        public override void SetMapping(EntityTypeBuilder<TLoginModel> mapBuilder)
        {
            mapBuilder.HasKey(x => x.Id);
            mapBuilder.Property(x => x.Id).ValueGeneratedOnAdd();


            mapBuilder.Property(x => x.CreationDateTime).IsRequired(true);
            mapBuilder.Property(x => x.ExpireDateTime).IsRequired(true);

            mapBuilder.HasOne(p => p.User)
                .WithMany()
                .IsRequired();

            //mapBuilder.Property(x => x.User).IsRequired(true);
        }
    }
}
