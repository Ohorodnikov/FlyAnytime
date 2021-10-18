using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb.Validation
{
    public interface IValidator<TEntity>
        where TEntity : IMongoEntity
    {
        bool Validate(TEntity entity, EntityErrorModel<TEntity> modelError);

        (bool isValid, EntityErrorModel<TEntity> errorModel) IsValid(TEntity entity);
    }

    public abstract class Validator<TEntity> : IValidator<TEntity>
        where TEntity : IMongoEntity
    {
        public (bool isValid, EntityErrorModel<TEntity> errorModel) IsValid(TEntity entity)
        {
            var errModel = new EntityErrorModel<TEntity>();

            var isValid = Validate(entity, errModel);

            return (isValid: isValid, errorModel: errModel);
        }

        public abstract bool Validate(TEntity entity, EntityErrorModel<TEntity> modelError);
    }
}
