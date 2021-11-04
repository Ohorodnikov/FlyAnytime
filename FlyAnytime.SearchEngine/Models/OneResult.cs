using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Models
{
    public class OneResult
    {
        public string CityFrom { get; set; }
        public string CityTo { get; set; }

        public long DateTimeFrom { get; set; }
        public long DateTimeBack { get; set; }

        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }

        public string ResultUrl { get; set; }
    }
}
