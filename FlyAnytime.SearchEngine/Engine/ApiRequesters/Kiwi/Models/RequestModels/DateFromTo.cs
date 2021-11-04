using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// search flights from this date (dd/mm/yyyy). Use parameters date_from and date_to as a date range for the flight departure.
    ///Parameters 'date_from=01/04/2021' and 'date_to=01/04/2021' mean that the departure can be anytime between the specified dates.
    ///For the dates of the return flights, use the 'return_to' and 'return_from' or 'nights_in_dst_from' and 'nights_in_dst_to' parameters.
    /// </summary>
    public class DateFrom : BaseDateTimeParam
    {
        public DateFrom(DateTime value) : base("date_from", value) { }
    }

    /// <summary>
    /// search flights upto this date (dd/mm/yyyy)
    /// </summary>
    public class DateTo : BaseDateTimeParam
    {
        public DateTo(DateTime value) : base("date_to", value) { }
    }
}
