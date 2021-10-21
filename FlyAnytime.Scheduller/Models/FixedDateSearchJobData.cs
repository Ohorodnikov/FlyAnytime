using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Scheduler.Models
{
    public class FixedDateSearchJobData : BaseSearchJobData
    {
        public long StartDate { get; set; }
        public long EndDate { get; set; }
    }

    public class FixedDateSearchJobDataMapping : BaseJobDataMap<FixedDateSearchJobData>
    {
        public FixedDateSearchJobDataMapping() : base("FixDateSearch")
        {
        }

        public override void SetMapping(EntityTypeBuilder<FixedDateSearchJobData> mapBuilder)
        {
            base.SetMapping(mapBuilder);
        }
    }
}
