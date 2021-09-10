using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MongoDb
{
    public interface IMongoEntity
    {
        object Id { get; set; }
    }

    public interface IMongoRootEntity : IMongoEntity
    {
        ObjectId Id { get; set; }
    }

    public abstract class MongoRoot : IMongoRootEntity
    {
        public ObjectId Id { get; set; }
        object IMongoEntity.Id { get => Id; set => Id = (ObjectId)value; }
    }

    public interface IMongoInternalEntity : IMongoEntity
    {
        Guid Id { get; set; }
    }

    public abstract class MongoInternalEntity : IMongoInternalEntity
    {
        public Guid Id { get; set; }
        object IMongoEntity.Id { get => Id; set => Id = (Guid)value; }
    }

}
