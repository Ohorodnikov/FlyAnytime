using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using FlyAnytime.Tools;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : IMongoRootEntity
    {
        IMongoDbContext _dbContext;
        IValidator<TEntity> _validator;
        public Repository(
            IMongoDbContext dbContext, 
            IValidator<TEntity> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<TEntity> GetById(ObjectId id)
        {
            return await _dbContext
                .Set<TEntity>()
                .Find(ent => ent.Id == id)
                .FirstOrDefaultAsync();
        }

        private void SetIdsForInternalEntities<TEnt>(TEnt entity)
            where TEnt : IMongoEntity
        {
            var props = entity.GetType().GetAllPropsOfType(typeof(IMongoInternalEntity));

            foreach (var prop in props)
            {
                var value = (IMongoInternalEntity)prop.GetValue(entity);
                if (value != null)
                    value.Id = Guid.NewGuid();                

                SetIdsForInternalEntities(value);
            }
        }

        public async Task<(bool success, EntityErrorModel errorModel)> TryCreate(TEntity entity)
        {
            SetIdsForInternalEntities(entity);
            if (_validator != null)
            {
                var (isValid, errorModel) = _validator.IsValid(entity);
                if (!isValid)
                {
                    return (success: false, errorModel: errorModel);
                }
            }

            try
            {
                await _dbContext.Set<TEntity>().InsertOneAsync(entity);
                return (success: true, errorModel: null);
            }
            catch (Exception e)
            {
                var errModel = new EntityErrorModel(e);
                return (success: false, errorModel: errModel);
            }
        }
    }
}
