using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb.Mapping
{
    public interface IMongoInternalEntityMap<TEntity> : IMongoEntityMap<TEntity>
        where TEntity : IMongoInternalEntity
    {

    }

    public abstract class InternalEntityMap<TEntity> : BaseEntityMap<TEntity>
        where TEntity : IMongoInternalEntity
    {

    }
}
