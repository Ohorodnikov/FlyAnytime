using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FlyAnytime.Tools
{
    public static class DateTimeHelper
    {
        public static long UnixNow => UtcToUnix(DateTime.UtcNow);

        public static long ToUtcUnix(this DateTime dateTime) => UtcToUnix(dateTime.ToUniversalTime());

        public static DateTime UnixToUtc(long unixTime) => DateTime.UnixEpoch.AddSeconds(unixTime);

        private static long UtcToUnix(DateTime dateTime) => (long)dateTime.Subtract(DateTime.UnixEpoch).TotalSeconds;

        public static DateTime IsoToDateTime(string isoDateTime) => DateTime.Parse(isoDateTime, null, DateTimeStyles.RoundtripKind);

        public static long IsoToUnix(string isoDateTime) => UtcToUnix(IsoToDateTime(isoDateTime));

    }
}
