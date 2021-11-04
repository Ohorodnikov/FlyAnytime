using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels
{
    public class SearchResult
    {
        [JsonProperty("search_id")]
        public string Id { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("fx_rate")]
        public decimal FxRate { get; set; }

        [JsonProperty("data")]
        public List<SearchOneResult> Results { get; set; }
    }
}
