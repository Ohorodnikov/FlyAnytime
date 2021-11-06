using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IntegrationTests.Models
{
    public partial class Airports
    {
        [JsonProperty("locations")]
        public LocationElement[] Locations { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("last_refresh")]
        public long LastRefresh { get; set; }

        [JsonProperty("results_retrieved")]
        public long ResultsRetrieved { get; set; }
    }

    public partial class LocationElement
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("int_id")]
        public long IntId { get; set; }

        [JsonProperty("airport_int_id")]
        public long AirportIntId { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("icao")]
        public string Icao { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("slug_en")]
        public string SlugEn { get; set; }

        [JsonProperty("alternative_names")]
        public string[] AlternativeNames { get; set; }

        [JsonProperty("rank")]
        public long Rank { get; set; }

        [JsonProperty("global_rank_dst")]
        public long GlobalRankDst { get; set; }

        [JsonProperty("dst_popularity_score")]
        public long DstPopularityScore { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("city")]
        public City_Kiwi City { get; set; }

        [JsonProperty("location")]
        public LocationLocation Location { get; set; }

        [JsonProperty("alternative_departure_points")]
        public AlternativeDeparturePoint[] AlternativeDeparturePoints { get; set; }

        [JsonProperty("tags")]
        public Tag[] Tags { get; set; }

        [JsonProperty("providers")]
        public long[] Providers { get; set; }

        [JsonProperty("special")]
        public Continent[] Special { get; set; }

        [JsonProperty("tourist_region")]
        public Continent[] TouristRegion { get; set; }

        [JsonProperty("car_rentals")]
        public CarRental[] CarRentals { get; set; }

        [JsonProperty("new_ground")]
        public bool NewGround { get; set; }

        [JsonProperty("routing_priority")]
        public long RoutingPriority { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class AlternativeDeparturePoint
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }
    }

    public partial class CarRental
    {
        [JsonProperty("provider_id")]
        public long ProviderId { get; set; }

        [JsonProperty("providers_locations")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public long[] ProvidersLocations { get; set; }
    }

    public partial class City_Kiwi
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("subdivision")]
        public object Subdivision { get; set; }

        [JsonProperty("autonomous_territory")]
        public object AutonomousTerritory { get; set; }

        [JsonProperty("country")]
        public Continent Country { get; set; }

        [JsonProperty("continent")]
        public Continent Continent { get; set; }

        [JsonProperty("region")]
        public Continent Region { get; set; }
    }

    public partial class Continent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
    }

    public partial class LocationLocation
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
    }

    public partial class Tag
    {
        [JsonProperty("tag")]
        public string TagTag { get; set; }

        [JsonProperty("month_to")]
        public long MonthTo { get; set; }

        [JsonProperty("month_from")]
        public long MonthFrom { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("locale")]
        public Locale Locale { get; set; }
    }

    public partial class Locale
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DecodeArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long[]);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            var value = new List<long>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                var converter = ParseStringConverter.Singleton;
                var arrayItem = (long)converter.ReadJson(reader, typeof(long), null, serializer);
                value.Add(arrayItem);
                reader.Read();
            }
            return value.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (long[])untypedValue;
            writer.WriteStartArray();
            foreach (var arrayItem in value)
            {
                var converter = ParseStringConverter.Singleton;
                converter.WriteJson(writer, arrayItem, serializer);
            }
            writer.WriteEndArray();
            return;
        }

        public static readonly DecodeArrayConverter Singleton = new DecodeArrayConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }


    
}


