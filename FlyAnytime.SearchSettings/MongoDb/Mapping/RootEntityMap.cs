using FlyAnytime.Tools;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb.Mapping
{
    public interface IMongoRootEntityMap : IMongoEntityMap
    {
        string TableName { get; }

    }
    public interface IMongoRootEntityMap<TEntity> : IMongoEntityMap<TEntity>, IMongoRootEntityMap
        where TEntity : IMongoRootEntity
    {
    }

    public abstract class RootEntityMap<TEntity> : BaseEntityMap<TEntity>, IMongoRootEntityMap<TEntity>
        where TEntity : IMongoRootEntity
    {
        public RootEntityMap(IMongoDbContext dbContext, string tableName)
        {
            TableName = tableName;
            DbContext = dbContext;
        }

        protected IMongoDbContext DbContext { get; }

        protected void AddUniqueIndex<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            var prop = expression.GetStringBody();

            var indexes = DbContext.Set<TEntity>().Indexes;
            var uniqueIndex = new CreateIndexOptions
            {
                Unique = true,
                Name = $"{typeof(TEntity).Name}_{prop}_Unique"
            };

            var indexModel = new CreateIndexModel<TEntity>($"{{ {prop}: 1 }}", uniqueIndex);
            indexes.CreateOne(indexModel);
        }

        public string TableName { get; }
    }
}
