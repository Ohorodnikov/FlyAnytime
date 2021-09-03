using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.EfContextBase
{
    public interface IEntityMap
    {
        void DoMap(ModelBuilder modelBuilder);
        bool IsMapped();
    }
    public interface IEntityMap<TEntity, TId> : IEntityMap
        where TEntity : class, IEntity<TId>
    {
        void SetMapping(EntityTypeBuilder<TEntity> mapBuilder);
    }
    public abstract class EntityMap<TEntity, TId> : IEntityMap<TEntity, TId>
        where TEntity : class, IEntity<TId>
    {
        protected string TableName { get; }
        public EntityMap(string tableName)
        {
            TableName = tableName;
        }

        public abstract void SetMapping(EntityTypeBuilder<TEntity> mapBuilder);
        public void DoMap(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<TEntity>();
            builder.ToTable(TableName);
            builder.HasKey(x => x.Id);

            SetMapping(builder);
        }

        public virtual bool IsMapped() => true;
    }
}
