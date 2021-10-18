using FlyAnytime.SearchSettings.MongoDb.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Repository
{
    public interface IMongoRepoResult
    {
        bool Success { get; }
        EntityErrorModel ErrorModel { get; }
        object Entity { get; }
    }

    public interface IMongoRepoResult<TEntity> : IMongoRepoResult
    {
        TEntity Entity { get; }
    }

    public class MongoRepoResult<TEntity> : IMongoRepoResult<TEntity>
    {
        public MongoRepoResult(EntityErrorModel errorModel)
        {
            Success = false;
            ErrorModel = errorModel;
        }
        public MongoRepoResult(TEntity entity)
        {
            Entity = entity;
            Success = true;
        }

        public bool Success { get; }
        public EntityErrorModel ErrorModel { get; }
        public TEntity Entity { get; }

        object IMongoRepoResult.Entity => Entity;
    }
}
