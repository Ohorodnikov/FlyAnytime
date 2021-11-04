using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels
{
    public class Duration
    {
        [JsonProperty("departure")]
        public long DepartureInSec { get; set; }

        [JsonProperty("return")]
        public long ReturnInSec { get; set; }

        [JsonProperty("total")]
        public long TotalInSec { get; set; }
    }
}
