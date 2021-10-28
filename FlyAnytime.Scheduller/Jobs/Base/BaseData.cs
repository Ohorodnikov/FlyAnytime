using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Scheduler.Jobs.Base
{
    public abstract class BaseData : JobDataMap
    {
        public BaseData()
        {

        }

        public BaseData(JobDataMap map) : base((IDictionary<string, object>)map)
        {

        }
    }
}
