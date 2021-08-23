using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public class LoginTelegramApprove : LoginWithLimitedTime
    {
    }

    public class LoginTelegramApproveMapping : LoginWithLimitedTimeMapping<LoginTelegramApprove>
    {
        public LoginTelegramApproveMapping() : base("LTA") { }
    }
}
