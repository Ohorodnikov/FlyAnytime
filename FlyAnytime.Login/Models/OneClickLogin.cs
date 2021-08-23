using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public class OneClickLogin : LoginWithLimitedTime
    {
        public virtual string LoginUrl { get; set; }
    }

    public class OneClickLoginMapping : LoginWithLimitedTimeMapping<OneClickLogin>
    {
        public OneClickLoginMapping() : base("OCL") { }

        public override void SetMapping(EntityTypeBuilder<OneClickLogin> mapBuilder)
        {
            base.SetMapping(mapBuilder);

            mapBuilder.Property(x => x.LoginUrl).IsRequired(true);

        }
    }
}
