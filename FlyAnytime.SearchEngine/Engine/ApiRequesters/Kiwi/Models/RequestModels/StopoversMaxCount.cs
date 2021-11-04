using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// max number of stopovers per itinerary. Use 'max_stopovers=0' for direct flights only.
    /// </summary>
    public class StopoversMaxCount : BaseNumericParam
    {
        public StopoversMaxCount(long value) : base("max_stopovers", value) { }
    }

    /// <summary>
    /// result filter, max length of stopover, 48:00 means 2 days (48 hours)
    /// </summary>
    public class StopoversMaxLengthInHours : BaseNumericParam
    {
        public StopoversMaxLengthInHours(long value) : base("stopover_to", value) { }

        public override string ToSearchString() => $"{ParamValue}:00";
    }

    /// <summary>
    /// result filter, min length of stopover, 48:00 means 2 days (48 hours)
    /// </summary>
    public class StopoversMinLengthInHours : BaseNumericParam
    {
        public StopoversMinLengthInHours(long value) : base("stopover_from", value) { }
        public override string ToSearchString() => $"{ParamValue}:00";
    }
}
