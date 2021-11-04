using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters
{
    public class ApiResultModel
    {
        public string CityCodeFrom { get; set; }
        public string CityCodeTo { get; set; }

        public int StepoverCountTo { get; set; }
        public int StepoverCountBack { get; set; }
        public decimal Price { get; set; }
        public decimal PriceInEur { get; set; }
        public string Currency { get; set; }

        public long ArrivalDateTimeToDestinationUtc { get; set; }
        public long BackDateTimeFromDestinationUtc { get; set; }

        public long DepartureFromDateTimeLocal { get; set; }
        public long ArrivalBackDateTimeLocal { get; set; }

        public string LinkOnResult { get; set; }
    }
}
