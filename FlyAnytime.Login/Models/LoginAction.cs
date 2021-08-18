using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public class LoginAction
    {
        public Guid Id { get; set; }
        public OneClickLogin OneClickLogin { get; set; }
        public User User { get; set; }
    }
}
