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
    public class Schedule : MongoInternalEntity
    {
        public bool IsActive { get; set; }
        public ScheduleIntervalType Type { get; set; }
        public byte Interval { get; set; }

        /// <summary>
        /// Available if type is Day
        /// </summary>
        public byte Hour { get; set; }
        public byte Minute { get; set; }

        public string CustomCronSchedule { get; set; }
    }

    public class ScheduleMap : InternalEntityMap<Schedule>
    {
        public override void SetCustomMapping(BsonClassMap<Schedule> classMap)
        {
        }
    }
}
