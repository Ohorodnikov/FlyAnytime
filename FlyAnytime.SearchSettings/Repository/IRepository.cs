using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Repository
{
    public interface IRepository<TEntity>
        where TEntity : IMongoRootEntity
    {
        Task<TEntity> GetById(ObjectId id);
        Task<(bool success, EntityErrorModel errorModel)> TryCreate(TEntity entity);
    }
}
