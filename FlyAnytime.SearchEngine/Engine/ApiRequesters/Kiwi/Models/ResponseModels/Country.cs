using Newtonsoft.Json;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels
{
    public class Country
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
