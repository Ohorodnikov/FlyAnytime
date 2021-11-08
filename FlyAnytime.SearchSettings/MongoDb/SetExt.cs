using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb
{
    public static class SetExt
    {
        public static async Task RemoveAll<TEntity>(this IMongoCollection<TEntity> set)
            where TEntity : IMongoRootEntity
        {
            await set.DeleteManyAsync(x => true);
        }
    }
}
