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
    public class City : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public ObjectId CountryId { get; set; }
        public Country Country { get; set; }

        public IEnumerable<LocalizationItem> Localizations { get; set; }
    }

    public class CityMap : RootEntityMap<City>
    {
        public CityMap() : base("City")
        {
        }

        public override void SetMapping(BsonClassMap<City> classMap)
        {

        }
    }
}
