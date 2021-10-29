using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using FlyAnytime.Tools;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
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

            return WrapAndReturn(entity);
        }

        public async Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> propExpr)
        {
            var data = await Set.FindAsync(propExpr);

            return await data.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllBy(string propName, string value)
        {
            var flt = Builders<TEntity>.Filter;

            var filter = flt.Eq(propName, value);

            return await Set
                .Find(filter)
                .ToListAsync();
        }

        public async Task<IMongoRepoResult<TEntity>> GetOneBy(Expression<Func<TEntity, bool>> propExpr)
        {
            var e = await Set.FindAsync(propExpr);

            var ent = await e.FirstOrDefaultAsync();

            return WrapAndReturn(ent);
        }

        public async Task<IMongoRepoResult<TEntity>> GetOneBy(string propName, string value)
        {
            var flt = Builders<TEntity>.Filter;

            var filter = flt.Eq(propName, value);

            var entity = await Set
                .Find(filter)
                .FirstOrDefaultAsync();

            return WrapAndReturn(entity);
        }

        private MongoRepoResult<TEntity> WrapAndReturn(TEntity entity)
        {
            if (entity == null)
            {
                var errModel = new EntityErrorModel<TEntity>();
                errModel.AddValidationError(x => x.Id, "Entity was not found!");

                return new MongoRepoResult<TEntity>(errModel);
            }

            return new MongoRepoResult<TEntity>(entity);
        }

        private void PrepareToSave<TEnt>(TEnt entity)
            where TEnt : IMongoEntity
        {
            SetIdsForInternalEntities(entity);
            SetIdsForReferenceRoot(entity);
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
            where TEnt : IMongoEntity
        {
            var entType = entity.GetType();
            var rootProps = entType.GetAllPropsOfType(typeof(IMongoRootEntity));

            PropertyInfo GetIdProp(Type entityOwnerType, PropertyInfo propertyOnRoot)
            {
                var isList = propertyOnRoot.PropertyType.IsImplementInterface(typeof(IEnumerable));

                var idPropName = isList ? $"{propertyOnRoot.Name}Ids" : $"{propertyOnRoot.Name}Id";

                return entityOwnerType.GetProperty(idPropName);
            }

            foreach (var prop in rootProps)
            {
                var v = prop.GetValue(entity);
                var idProp = GetIdProp(entType, prop);
                if (v == null)
                {
                    continue;
                    //idProp.SetValue(entity, idProp.PropertyType.GetDefaultValue()); //Impossible to detect if relation was set to null or just dont load
                }
                else if (v is IMongoRootEntity root)
                {
                    idProp.SetValue(entity, root.Id);
                }
                else if (v is IEnumerable<IMongoRootEntity> rootList)
                {
                    var idList = new List<ObjectId>(rootList.Count());
                    foreach (var rootItem in rootList)
                        idList.Add(rootItem.Id);

                    idProp.SetValue(entity, idList);
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

            PrepareToSave(entity);

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

            PrepareToSave(entity);

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
