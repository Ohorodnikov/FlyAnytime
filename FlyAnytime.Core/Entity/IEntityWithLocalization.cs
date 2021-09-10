using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.Entity
{
    public interface IEntityWithLocalization : IEntity
    {
        string TypeDescriptor { get; }
    }
}
