using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.Entity
{
    public interface IEntity
    {
        object Id { get; set; }
    }
    public class Entity<TId> : IEntity
    {
        public virtual TId Id { get; set; }
        object IEntity.Id { get => Id; set => Id = (TId)value; }
    }
}
