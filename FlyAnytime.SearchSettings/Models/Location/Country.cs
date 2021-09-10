using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyAnytime.SearchSettings.Models.Location
{
    public class Country : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public IEnumerable<LocalizationItem> Localizations { get; set; }
    }

    public class CountryMap : RootEntityMap<Country>
    {
        public CountryMap() : base("Country")
        {
        }

        public override void SetMapping(BsonClassMap<Country> classMap)
        {

        }
    }
}
