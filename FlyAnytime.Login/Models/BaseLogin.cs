﻿using FlyAnytime.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Models
{
    public abstract class BaseLogin : Entity<long>
    {
        public virtual User User { get; set; }
    }
}
