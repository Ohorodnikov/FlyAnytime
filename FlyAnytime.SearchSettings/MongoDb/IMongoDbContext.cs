using FlyAnytime.Core;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb
{
    public interface IMongoDbContext : IDbContextBase
    {
        IMongoCollection<TEntity> Set<TEntity>()
            where TEntity : IMongoRootEntity;

        //Task InitDatabase();

        void DoMap();
    }

    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _db;
        private readonly IServiceProvider _serviceProvider;
        public MongoDbContext(MongoSettings settings, IServiceProvider serviceProvider)
        {
            var conn = new MongoClient(settings.Connection);
            _db = conn.GetDatabase(settings.DbName);

            _serviceProvider = serviceProvider;
        }

        public IMongoCollection<TEntity> Set<TEntity>()
            where TEntity : IMongoRootEntity
        {
            var map = _serviceProvider.GetService<IMongoRootEntityMap<TEntity>>();

            return _db.GetCollection<TEntity>(map.TableName);
        }

        private static bool _inited = false;
        public async Task ReCreateDb()
        {
            var allMaps = _serviceProvider.GetServices<IMongoRootEntityMap>().Select(x => x.TableName);
            await EnsureDeleted(allMaps);

            DoMap();

            await EnsureCreated(allMaps);
        }

        private async Task EnsureDeleted(IEnumerable<string> allTablesName)
        {
            foreach (var table in allTablesName)
                await _db.DropCollectionAsync(table);
        }

        private async Task EnsureCreated(IEnumerable<string> allTablesName)
        {
            var currentTables = await _db.ListCollectionNamesAsync();
            var currentTablesNames = await currentTables.ToListAsync();

            foreach (var table in allTablesName)
                if (!currentTablesNames.Contains(table))
                    await _db.CreateCollectionAsync(table);
        }

        public void DoMap()
        {
            if (_inited)
                return;

            var allMaps = _serviceProvider.GetServices<IMongoEntityMap>();

            foreach (var map in allMaps)
            {
                if (BsonClassMap.IsClassMapRegistered(map.EntityType))
                {
                    throw new Exception($"Type {map.EntityType} has been already registered!");
                }

                var mapType = typeof(BsonClassMap<>).MakeGenericType(map.EntityType);

                var mapper = (BsonClassMap)Activator.CreateInstance(mapType);

                map.DoMap(mapper);

                BsonClassMap.RegisterClassMap(mapper);

                mapper.Freeze();

                map.AfterMap(mapper);
            }

            _inited = true;
        }
    }
}
