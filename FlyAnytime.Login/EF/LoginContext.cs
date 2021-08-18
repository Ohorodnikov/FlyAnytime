using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.EF
{
    public class LoginContext : BaseEfContext<LoginContext>
    {
        public LoginContext(DbContextOptions<LoginContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}
