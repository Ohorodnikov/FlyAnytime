using FlyAnytime.Core.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Scheduler.Models
{
    public class DynamicDateSearchJobData : BaseSearchJobData
    {
        public int DaysFromNowStart { get; set; }
        public int DaysFromNowEnd { get; set; }

        public string AllowedDateTimeSlotsTo { get; set; }
        public string AllowedDateTimeSlotsBack { get; set; }

        //public Dictionary<Days, HashSet<byte>> AllowedDateTimeSlotsTo { get; private set; }
        //public Dictionary<Days, HashSet<byte>> AllowedDateTimeSlotsBack { get; private set; }
    }

    public static class DynamicDateSearchJobDataExt
    {
        public static Dictionary<Days, HashSet<byte>> GetAllowedDateTimeSlotsTo(this DynamicDateSearchJobData data)
        {
            return GetAllowedDateTimeSlots(data.AllowedDateTimeSlotsTo);
        }

        public static Dictionary<Days, HashSet<byte>> GetAllowedDateTimeSlotsBack(this DynamicDateSearchJobData data)
        {
            return GetAllowedDateTimeSlots(data.AllowedDateTimeSlotsBack);
        }

        public static Dictionary<Days, HashSet<byte>> GetAllowedDateTimeSlots(string slots)
        {
            return JsonConvert.DeserializeObject<Dictionary<Days, HashSet<byte>>>(slots);
        }
    }

    public class DynamicDateSearchJobDataMapping : BaseJobDataMap<DynamicDateSearchJobData>
    {
        public DynamicDateSearchJobDataMapping() : base("DynDateSearch")
        {
        }

        public override void SetMapping(EntityTypeBuilder<DynamicDateSearchJobData> mapBuilder)
        {
            base.SetMapping(mapBuilder);
        }
    }
}
