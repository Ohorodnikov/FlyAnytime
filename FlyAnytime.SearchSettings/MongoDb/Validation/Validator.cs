using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb.Validation
{
    public interface IValidator<TEntity>
        where TEntity : IMongoEntity
    {
        bool Validate(TEntity entity, EntityErrorModel modelError);

        (bool isValid, EntityErrorModel errorModel) IsValid(TEntity entity);
    }

    public abstract class Validator<TEntity> : IValidator<TEntity>
        where TEntity : IMongoEntity
    {
        public (bool isValid, EntityErrorModel errorModel) IsValid(TEntity entity)
        {
            var errModel = new EntityErrorModel();

            var isValid = Validate(entity, errModel);

            return (isValid: isValid, errorModel: errModel);
        }

        public abstract bool Validate(TEntity entity, EntityErrorModel modelError);
    }
}
