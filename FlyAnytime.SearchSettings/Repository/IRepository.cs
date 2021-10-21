using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Repository
{
    public interface IRepository<TEntity>
        where TEntity : IMongoRootEntity
    {
        IMongoCollection<TEntity> Set { get; }
        long Count { get; }

        Task<IMongoRepoResult<TEntity>> GetById(ObjectId id);
        Task<IMongoRepoResult<TEntity>> GetBy(string propName, string value);
        Task<IMongoRepoResult<TEntity>> GetBy(Expression<Func<TEntity, object>> propExpr, string value);


        Task<IMongoRepoResult<TEntity>> TryCreate(TEntity entity);

        Task<IMongoRepoResult<TEntity>> TryReplace(TEntity entity);

        Task<IMongoRepoResult<TEntity>> TryDelete(ObjectId id);

        Task<IEnumerable<TEntity>> GetNext(int skip, int take);
    }
}
