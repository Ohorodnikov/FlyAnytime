using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// the minimal length of stay in the destination given in the fly_to parameter.
    /// </summary>
    public class NightsInDestFrom : BaseNumericParam
    {
        public NightsInDestFrom(long value) : base("nights_in_dst_from", value) { }
    }

    /// <summary>
    /// the maximal length of stay in the destination given in the fly_to parameter.
    ///Either both parameters 'nights_in_dst_to' and 'nights_in_dst_from' have to be specified or none of them.
    /// </summary>
    public class NightsInDestTo : BaseNumericParam
    {
        public NightsInDestTo(long value) : base("nights_in_dst_to", value) { }
    }

    public static class NightsInDestHelper
    {
        public static IRequestDescriptor AddNightsInDestination(this IRequestDescriptor setter, int minDays, int maxDays)
        {
            if (minDays <= 0 || maxDays <= 0 || maxDays < minDays)
                throw new ArgumentOutOfRangeException($"{nameof(maxDays)} must be greater than {nameof(minDays)}");

            setter.AddParam(new NightsInDestFrom(minDays));
            setter.AddParam(new NightsInDestTo(maxDays));

            return setter;
        }
    }
}
