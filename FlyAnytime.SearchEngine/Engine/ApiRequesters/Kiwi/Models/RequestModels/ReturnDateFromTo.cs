using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// min return date of the whole trip (dd/mm/yyyy)
    /// </summary>
    public class ReturnDateFrom : BaseDateTimeParam
    {
        public ReturnDateFrom(DateTime value) : base("return_from", value) { }
    }

    /// <summary>
    /// max return date of the whole trip (dd/mm/yyyy)
    /// </summary>
    public class ReturnDateTo : BaseDateTimeParam
    {
        public ReturnDateTo(DateTime value) : base("return_to", value) { }
    }
}
