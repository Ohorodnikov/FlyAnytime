using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels
{
    public class SearchOneResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("deep_link")]
        public string Link { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("pnr_count")]
        public int Passengers { get; set; }

        [JsonProperty("has_airport_change")]
        public bool HasAirportChange { get; set; }

        [JsonProperty("technical_stops")]
        public int TechnicalStops { get; set; }

        [JsonProperty("throw_away_ticketing")]
        public bool ThrowAwayTicketing { get; set; }

        [JsonProperty("hidden_city_ticketing")]
        public bool HiddenCityTicketing { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("facilitated_booking_available")]
        public bool FacilitatedBookingAvailable { get; set; }

        [JsonProperty("quality")]
        public double Quality { get; set; }

        [JsonProperty("virtual_interlining")]
        public bool VirtualInterlining { get; set; }

        [JsonProperty("booking_token")]
        public string BookToken { get; set; }

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

        [JsonProperty("local_departure")]
        public string LocalDeparture { get; set; }

        [JsonProperty("utc_departure")]
        public string UtcDeaprture { get; set; }

        [JsonProperty("local_arrival")]
        public string LocalArrival { get; set; }

        [JsonProperty("utc_arrival")]
        public string UtcArrival { get; set; }


        [JsonProperty("duration")]
        public Duration Duration { get; set; }

        [JsonProperty("countryFrom")]
        public Country CountryFrom { get; set; }

        [JsonProperty("countryTo")]
        public Country CountryTo { get; set; }

        [JsonProperty("routes")]
        public List<List<string>> Routes { get; set; }

        [JsonProperty("airlines")]
        public List<string> AirlinesCodes { get; set; }

        [JsonProperty("bags_price")]
        public BagPrice BagPrice { get; set; }

        [JsonProperty("baglimit")]
        public BagLimits BagLimit { get; set; }

        [JsonProperty("route")]
        public List<Route> Route { get; set; }
    }
}
