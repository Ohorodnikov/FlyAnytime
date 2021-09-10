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
    public enum SearchDateSettingsType
    {
        FixedRange = 1,
        DynamicRange = 2
    }

    public enum Days
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }

    public class DateSettings : MongoInternalEntity
    {
        public SearchDateSettingsType Type { get; set; }

        public int DaysCountMin { get; set; }
        public int DaysCountMax { get; set; }

        public FixedDateSettings FixedDateSettings { get; set; }
        public DynamicDateSettings DynamicDateSettings { get; set; }
    }

    public class DateSettingsMap : InternalEntityMap<DateSettings>
    {
        public override void SetMapping(BsonClassMap<DateSettings> classMap)
        {
        }
    }

    public class FixedDateSettings : MongoInternalEntity
    {
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
    }

    public class FixedDateSettingsMap : InternalEntityMap<FixedDateSettings>
    {
        public override void SetMapping(BsonClassMap<FixedDateSettings> classMap)
        {
        }
    }

    public class DynamicDateSettings : MongoInternalEntity
    {
        public int DatesFromNowStart { get; set; }
        public int DatesFromNowEnd { get; set; }

        public IEnumerable<FlyDaySettings> DepartureFlySettings { get; set; }
        public IEnumerable<FlyDaySettings> ReturnFlySettings { get; set; }
    }

    public class DynamicDateSettingsMap : InternalEntityMap<DynamicDateSettings>
    {
        public override void SetMapping(BsonClassMap<DynamicDateSettings> classMap)
        {
        }
    }

    public class FlyDaySettings : MongoInternalEntity
    {
        public Days Day { get; set; }

        public byte AllowedHourStart { get; set; }
        public byte AllowedMinuteStart { get; set; }
        public byte AllowedHourEnd { get; set; }
        public byte AllowedMinuteEnd { get; set; }
    }

    public class FlyDaySettingsMap : InternalEntityMap<FlyDaySettings>
    {
        public override void SetMapping(BsonClassMap<FlyDaySettings> classMap)
        {
        }
    }
}
