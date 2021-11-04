using FlyAnytime.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    public enum FlyDaysType
    {
        Departure,
        Arrival
    }

    public enum FlyDays
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
    }

    public abstract class BaseFlyDays : BaseSearchParam<FlyDays>
    {
        public BaseFlyDays(string paramName, FlyDays value) : base(paramName, value) { }

        public BaseFlyDays(string paramName, Days value) : base(paramName, ConvertToFlyDays(value)) { }

        private static FlyDays ConvertToFlyDays(Days value)
        {
            return value switch
            {
                Days.Monday => FlyDays.Monday,
                Days.Tuesday => FlyDays.Tuesday,
                Days.Wednesday => FlyDays.Wednesday,
                Days.Thursday => FlyDays.Thursday,
                Days.Friday => FlyDays.Friday,
                Days.Saturday => FlyDays.Saturday,
                Days.Sunday => FlyDays.Sunday,
                _ => throw new NotImplementedException(),
            };
        }

        public override string ToSearchString() => ((int)ParamValue).ToString();
    }

    public abstract class BaseFlyDaysType : BaseSearchParam<FlyDaysType>
    {
        protected BaseFlyDaysType(string paramName, FlyDaysType value) : base(paramName, value) { }

        public override string ToSearchString()
        {
            return ParamValue switch
            {
                FlyDaysType.Departure => "departure",
                FlyDaysType.Arrival => "arrival",
                _ => throw new NotImplementedException()
            };
        }
    }

    /// <summary>
    /// the list of week days for the flight
    /// </summary>
    public class FlyDateFrom : BaseFlyDays
    {
        public FlyDateFrom(FlyDays value) : base("fly_days", value) { }

        public FlyDateFrom(Days value) : base("fly_days", value) { }
    }

    /// <summary>
    /// the list of week days for the flight
    /// </summary>
    public class FlyDateReturn : BaseFlyDays
    {
        public FlyDateReturn(FlyDays value) : base("ret_fly_days", value) { }

        public FlyDateReturn(Days value) : base("ret_fly_days", value) { }
    }

    /// <summary>
    /// type of set fly_days; It is used to specify whether the flight is an arrival or a departure.
    /// </summary>
    public class FlyDaysFromType : BaseFlyDaysType
    {
        public FlyDaysFromType(FlyDaysType value) : base("fly_days_type", value) { }
    }

    /// <summary>
    /// type of set ret_fly_days; It is used to specify whether the flight is an arrival or a departure.
    /// </summary>
    public class FlyDaysReturnType : BaseFlyDaysType
    {
        public FlyDaysReturnType(FlyDaysType value) : base("ret_fly_days_type", value) { }
    }
}
