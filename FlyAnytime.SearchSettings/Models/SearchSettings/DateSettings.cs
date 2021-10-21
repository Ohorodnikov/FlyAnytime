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
    public class DateSettings : MongoInternalEntity
    {
        public SearchDateSettingsType Type { get; set; }

        public int TripDaysCountMin { get; set; }
        public int TripDaysCountMax { get; set; }

        public FixedDateSettings FixedDateSettings { get; set; }
        public DynamicDateSettings DynamicDateSettings { get; set; }
    }

    public class FixedDateSettings : MongoInternalEntity
    {
        public long StartDateUtc { get; set; }
        public long EndDateUtc { get; set; }
    }

    public class DynamicDateSettings : MongoInternalEntity
    {
        public int DaysFromNowStart { get; set; }
        public int DaysFromNowEnd { get; set; }

        public IEnumerable<FlyDaySettings> DepartureFlySettings { get; set; }
        public IEnumerable<FlyDaySettings> ReturnFlySettings { get; set; }
    }

    public class FlyDaySettings : MongoInternalEntity
    {
        public Days Day { get; set; }

        public byte AllowedHourStart { get; set; }
        public byte AllowedHourEnd { get; set; }
    }

    public class DateSettingsMap : InternalEntityMap<DateSettings>
    {
        public override void SetCustomMapping(BsonClassMap<DateSettings> classMap)
        {
        }
    }

    public class FixedDateSettingsMap : InternalEntityMap<FixedDateSettings>
    {
        public override void SetCustomMapping(BsonClassMap<FixedDateSettings> classMap)
        {
        }
    }

    public class DynamicDateSettingsMap : InternalEntityMap<DynamicDateSettings>
    {
        public override void SetCustomMapping(BsonClassMap<DynamicDateSettings> classMap)
        {
        }
    }

    public class FlyDaySettingsMap : InternalEntityMap<FlyDaySettings>
    {
        public override void SetCustomMapping(BsonClassMap<FlyDaySettings> classMap)
        {
        }
    }
}
