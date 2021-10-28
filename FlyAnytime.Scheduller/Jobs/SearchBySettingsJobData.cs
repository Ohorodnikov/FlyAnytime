using FlyAnytime.Scheduler.Jobs.Base;
using Quartz;
using System.Collections.Generic;

namespace FlyAnytime.Scheduler.Jobs
{
    public class SearchBySettingsJobData : BaseData
    {
        public SearchBySettingsJobData()
        {

        }

        public SearchBySettingsJobData(JobDataMap map) : base(map)
        {

        }

        public long JobDataId
        {
            get
            {
                return GetLong(nameof(JobDataId));
            }
            set
            {
                Put(nameof(JobDataId), value);
            }
        }
    }
}
