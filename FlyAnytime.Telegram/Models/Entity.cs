using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
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
