using FlyAnytime.Tools;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb.Mapping
{
    public interface IMongoEntityMap
    {
        Type EntityType { get; }
        void DoMap(BsonClassMap mapper);
        void AfterMap(BsonClassMap mapper);
    }

    public interface IMongoEntityMap<TEntity> : IMongoEntityMap
        where TEntity : IMongoEntity
    {
        void SetCustomMapping(BsonClassMap<TEntity> classMap);
    }

    public abstract class BaseEntityMap<TEntity> : IMongoEntityMap<TEntity>
        where TEntity : IMongoEntity
    {
        public Type EntityType => typeof(TEntity);
        public abstract void SetCustomMapping(BsonClassMap<TEntity> classMap);
        public virtual void DoAfterMapping(BsonClassMap<TEntity> classMap)
        {

        }

        public void AfterMap(BsonClassMap mapper)
        {
            DoAfterMapping((BsonClassMap<TEntity>)mapper);
        }

        public void DoMap(BsonClassMap mapper)
        {
            mapper.AutoMap();

            UnmapPropertiesOnRoot(mapper);

            SetCustomMapping((BsonClassMap<TEntity>)mapper);
        }

        private void UnmapPropertiesOnRoot(BsonClassMap mapper)
        {
            foreach (var prop in typeof(TEntity).GetAllPropsOfType(typeof(IMongoRootEntity)))
                mapper.UnmapProperty(prop.Name);
        }
    }
}
