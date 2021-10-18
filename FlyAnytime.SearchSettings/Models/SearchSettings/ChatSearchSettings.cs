using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyAnytime.SearchSettings.Models.SearchSettings
{
    public class ChatSearchSettings : MongoInternalEntity
    {
        public IEnumerable<ChatSearchGroup> SearchGroups { get; set; }

        public PriceSettings PriceSettings { get; set; }

        public DateSettings DateSettings { get; set; }

        public bool IsActive { get; set; }
    }

    public class ChatSearchSettingsMap : InternalEntityMap<ChatSearchSettings>
    {
        public override void SetCustomMapping(BsonClassMap<ChatSearchSettings> classMap)
        {
        }
    }
}
