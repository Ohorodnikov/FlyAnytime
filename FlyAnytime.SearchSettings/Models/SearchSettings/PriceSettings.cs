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
    public enum SearchPriceSettingsType
    {
        FixPrice = 1,
        Percent = 2
    }

    public class PriceSettings : MongoInternalEntity
    {
        public SearchPriceSettingsType Type { get; set; }

        public decimal FixPriceAmount { get; set; }

        /// <summary>
        /// if PercentDiscountValue == 10, so show results if price is less than RegularPrice - 10%
        /// </summary>
        public decimal PercentDiscountValue { get; set; }
    }

    public class PriceSettingsMap : InternalEntityMap<PriceSettings>
    {
        public override void SetMapping(BsonClassMap<PriceSettings> classMap)
        {
        }
    }
}
