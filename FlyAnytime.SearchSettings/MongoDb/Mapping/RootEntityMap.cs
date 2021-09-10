using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public RootEntityMap(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; }
    }
}
