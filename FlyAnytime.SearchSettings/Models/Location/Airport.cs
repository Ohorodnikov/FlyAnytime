using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Models.Location
{
    public class Airport : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public ObjectId CityId { get; set; }
        public City City { get; set; }

        public IEnumerable<LocalizationItem> Localizations { get; set; }
    }

    public class AirportMap : RootEntityMap<Airport>
    {
        public AirportMap() : base("Airport")
        {
        }

        public override void SetMapping(BsonClassMap<Airport> classMap)
        {
            
        }
    }
}
