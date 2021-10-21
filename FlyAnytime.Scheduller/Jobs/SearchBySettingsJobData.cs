using Quartz;

namespace FlyAnytime.Scheduler.Jobs
{
    class SearchBySettingsJobData : JobDataMap
    {
        public long JobDataId { get; set; }
    }
}
