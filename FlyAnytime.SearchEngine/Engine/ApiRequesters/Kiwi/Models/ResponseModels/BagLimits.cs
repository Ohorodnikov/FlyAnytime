using Newtonsoft.Json;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels
{
    public class BagLimits
    {
        [JsonProperty("hand_width")]
        public int HandWidth { get; set; }

        [JsonProperty("hand_height")]
        public int HandHeight { get; set; }

        [JsonProperty("hand_length")]
        public int HandLength { get; set; }

        [JsonProperty("hand_weight")]
        public int HandWeight { get; set; }

        [JsonProperty("hold_width")]
        public int HoldWidth { get; set; }

        [JsonProperty("hold_height")]
        public int HoldHeight { get; set; }

        [JsonProperty("hold_length")]
        public int HoldLength { get; set; }

        [JsonProperty("hold_weight")]
        public int HoldWeight { get; set; }

        [JsonProperty("hold_dimensions_sum")]
        public int HoldDimSum { get; set; }
    }
}
