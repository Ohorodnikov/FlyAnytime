using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using FlyAnytime.Tools;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public IMongoCollection<TEntity> Set => _dbContext.Set<TEntity>();

        public long Count => Set.Find(x => true).CountDocuments();

        public async Task<IMongoRepoResult<TEntity>> GetById(ObjectId id)
        {
            var entity = await Set
                .Find(ent => ent.Id == id)
                .FirstOrDefaultAsync();


            if (entity == null)
            {
                var errModel = new EntityErrorModel<TEntity>();
                errModel.AddValidationError(x => x.Id, "Entity was not found!");

                return new MongoRepoResult<TEntity>(errModel);
            }

            return new MongoRepoResult<TEntity>(entity);
        }

        public async Task<IMongoRepoResult<TEntity>> GetBy(Expression<Func<TEntity, object>> propExpr, string value)
        {
            return await GetBy(propExpr.GetStringBody(), value);
        }

        public async Task<IMongoRepoResult<TEntity>> GetBy(string propName, string value)
        {
            var flt = Builders<TEntity>.Filter;

            var filter = flt.Eq(propName, value);

            var entity = await Set
                .Find(filter)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                var errModel = new EntityErrorModel<TEntity>();
                errModel.AddValidationError(x => x.Id, "Entity was not found!");

                return new MongoRepoResult<TEntity>(errModel);
            }

            return new MongoRepoResult<TEntity>(entity);
        }

        private void SetIdsForInternalEntities<TEnt>(TEnt entity)
            where TEnt : IMongoEntity
        {
            void SetIdImpl(IMongoInternalEntity e)
            {
                if (e != null && e.Id == default)
                    e.Id = Guid.NewGuid();

                SetIdsForInternalEntities(e);
            }

            var props = entity.GetType().GetAllPropsOfType(typeof(IMongoInternalEntity));

            foreach (var prop in props)
            {
                var v = prop.GetValue(entity);
                if (v is IMongoInternalEntity value)
                    SetIdImpl(value);
                else if (v is IEnumerable<IMongoInternalEntity> internalList)
                    foreach (var val in internalList)
                        SetIdImpl(val);
            }
        }

        private void SetIdsForReferenceRoot<TEnt>(TEnt entity)
        {
            var entType = entity.GetType();
            var rootProps = entType.GetAllPropsOfType(typeof(IMongoRootEntity));

            foreach (var prop in rootProps)
            {
                var v = prop.GetValue(entity);
                if (v == null)
                {
                    continue;
                }

                if (v is IMongoRootEntity root)
                {
                    var idProp = entType.GetProperty($"{prop.Name}Id");
                    idProp.SetValue(entity, root.Id);
                }
                else if (v is IEnumerable<IMongoRootEntity> rootList)
                {
                    var idProp = entType.GetProperty($"{prop.Name}Ids");

                    var idList = new List<ObjectId>(rootList.Count());
                    foreach (var rootItem in rootList)
                    {
                        idList.Add(rootItem.Id);
                    }
                }
            }

            var internalProps = entType.GetAllPropsOfType(typeof(IMongoInternalEntity));

            foreach (var intProp in internalProps)
            {
                var v = intProp.GetValue(entity);
                if (v is IMongoInternalEntity value)
                    SetIdsForReferenceRoot(value);
                else if (v is IEnumerable<IMongoInternalEntity> internalList)
                    foreach (var val in internalList)
                        SetIdsForReferenceRoot(val);
            }
        }

        private IMongoRepoResult<TEntity> Validate(TEntity entity)
        {
            if (_validator != null)
            {
                var (isValid, errorModel) = _validator.IsValid(entity);
                if (!isValid)
                    return new MongoRepoResult<TEntity>(errorModel);
            }

            return new MongoRepoResult<TEntity>(entity);
        }

        public async Task<IMongoRepoResult<TEntity>> TryCreate(TEntity entity)
        {
            if (entity == null)
            {
                var errModel = new EntityErrorModel<TEntity>();
                errModel.AddValidationError("Entity", "Entity is null");
                return new MongoRepoResult<TEntity>(errModel);
            }
            SetIdsForInternalEntities(entity);

            SetIdsForReferenceRoot(entity);

            var res = Validate(entity);
            if (!res.Success)
                return res;

            async Task<TEntity> _doSave()
            {
                await Set.InsertOneAsync(entity);

                return entity;
            }

            return await TryDoAction(_doSave);
        }

        public async Task<IMongoRepoResult<TEntity>> TryDelete(ObjectId id)
        {
            var getResult = await GetById(id);

            if (!getResult.Success)
                return getResult;

            async Task<TEntity> _doDelete()
            {
                await Set.DeleteOneAsync(x => x.Id == id);

                return getResult.Entity;
            }

            return await TryDoAction(_doDelete);
        }

        public async Task<IMongoRepoResult<TEntity>> TryReplace(TEntity entity)
        {
            var getResult = await GetById(entity.Id);

            if (!getResult.Success)
                return getResult;

            SetIdsForInternalEntities(entity);

            var res = Validate(entity);
            if (!res.Success)
                return res;

            async Task<TEntity> _doSave()
            {
                var ro = new ReplaceOptions { IsUpsert = false };

                var replaceResult = await Set.ReplaceOneAsync(x => x.Id == entity.Id, entity, ro);

                return entity;
            }

            return await TryDoAction(_doSave);
        }

        private async Task<IMongoRepoResult<TEntity>> TryDoAction(Func<Task<TEntity>> act)
        {
            IMongoRepoResult<TEntity> res;
            try
            {
                var entity = await act();

                res = new MongoRepoResult<TEntity>(entity);
            }
            catch (Exception e)
            {
                res = new MongoRepoResult<TEntity>(new EntityErrorModel(e));
            }

            return res;
        }

        public async Task<IEnumerable<TEntity>> GetNext(int skip, int take)
        {
            return await Set.Find(x => true).Skip(skip).Limit(take).ToListAsync();
        }
    }
}
