using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public abstract class LoginWithLimitedTime : BaseLogin
    {
        public DateTime CreationDateTime { get; set; }
        public DateTime ExpireDateTime { get; set; }
    }
}
