using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// max flight duration in hours, min value 0
    /// </summary>
    public class MaxFlyDuration : BaseNumericParam
    {
        public MaxFlyDuration(long value) : base("max_fly_duration", value)
        {
        }
    }
}
