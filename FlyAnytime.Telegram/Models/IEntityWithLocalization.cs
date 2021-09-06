using FlyAnytime.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public interface IEntityWithLocalization : IEntity
    {
        string TypeDescriptor { get; }
    }
}
