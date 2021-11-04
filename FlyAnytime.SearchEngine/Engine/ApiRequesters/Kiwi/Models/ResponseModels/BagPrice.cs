using Newtonsoft.Json;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels
{
    public class BagPrice
    {
        [JsonProperty("1")]
        public decimal BagPrice1 { get; set; }

        [JsonProperty("2")]
        public decimal BagPrice2 { get; set; }

        [JsonProperty("hand")]
        public decimal HandBagPrice { get; set; }
    }
}
