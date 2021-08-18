using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public class OneClickLogin : LoginWithLimitedTime
    {
        public string LoginUrl { get; set; }
    }
}
