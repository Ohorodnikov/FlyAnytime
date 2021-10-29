using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Models
{
    public class Settings
    {
        public string CityFlyFrom { get; set; }
        public IEnumerable<string> AirportsFlyTo { get; }

        public decimal Amount { get; }
        public string Currency { get; }

        public int DaysMin { get; }
        public int DaysMax { get; }
    }
}
