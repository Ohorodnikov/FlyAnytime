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

        public FlyDateTimeInfo FromDateTime { get; set; }
        public FlyDateTimeInfo ReturnDateTime { get; set; }

        public string LinkOnResult { get; set; }
    }

    public class FlyDateTimeInfo
    {
        public long StartUtc { get; set; }
        public long StartLocal { get; set; }

        public long EndUtc { get; set; }
        public long EndLocal { get; set; }
    }
}
