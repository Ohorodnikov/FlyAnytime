using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
