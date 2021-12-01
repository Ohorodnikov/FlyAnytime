using FlyAnytime.Core.Entity;
using FlyAnytime.Core.Enums;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Models.SearchSettings
{
    public class PriceSettings : MongoInternalEntity
    {
        public SearchPriceSettingsType Type { get; set; }

        /// <summary>
        /// If Type is FixPrice => stored 420 USD(EUR).
        /// If Type is PercentDiscount => stored between 0 and 100
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class PriceSettingsMap : InternalEntityMap<PriceSettings>
    {
        public override void SetCustomMapping(BsonClassMap<PriceSettings> classMap)
        {
        }
    }
}
