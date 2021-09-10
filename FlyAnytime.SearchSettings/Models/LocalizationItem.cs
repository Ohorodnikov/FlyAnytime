using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyAnytime.SearchSettings.Models
{
    public class LocalizationItem : MongoInternalEntity
    {
        public string Value { get; set; }

        public ObjectId LanguageId { get; set; }
        public Language Language { get; set; }
    }

    public class LocalizationItemMap : InternalEntityMap<LocalizationItem>
    {
        public override void SetMapping(BsonClassMap<LocalizationItem> classMap)
        {
        }
    }
}
