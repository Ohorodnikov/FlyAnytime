using Newtonsoft.Json;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels
{
    public class Route
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("combination_id")]
        public string CombinationId { get; set; }


        [JsonProperty("airline")]
        public string AirlineCode { get; set; }

        [JsonProperty("flight_no")]
        public int FlightNo { get; set; }

        [JsonProperty("vehicle_type")]
        public string VehicleType { get; set; }


        [JsonProperty("flyFrom")]
        public string AirportCodeFrom { get; set; }

        [JsonProperty("cityCodeFrom")]
        public string CityCodeFrom { get; set; }

        [JsonProperty("cityFrom")]
        public string CityNameFrom { get; set; }


        [JsonProperty("flyTo")]
        public string AirportCodeTo { get; set; }

        [JsonProperty("cityCodeTo")]
        public string CityCodeTo { get; set; }

        [JsonProperty("cityTo")]
        public string CityNameTo { get; set; }


        [JsonProperty("return")]
        public byte IsReturn { get; set; }

        [JsonProperty("fare_category")]
        public string FareCategory { get; set; }

        [JsonProperty("bags_recheck_required")]
        public bool BagsRecheckRequired { get; set; }

        [JsonProperty("guarantee")]
        public bool Guarantee { get; set; }


        [JsonProperty("local_departure")]
        public string LocalDeparture { get; set; }

        [JsonProperty("utc_departure")]
        public string UtcDeparture { get; set; }

        [JsonProperty("local_arrival")]
        public string LocalArrival { get; set; }

        [JsonProperty("utc_arrival")]
        public string UtcArrival { get; set; }
    }
}
