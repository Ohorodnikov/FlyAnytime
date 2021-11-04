namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// returns the cheapest flights for one date. Can be 0 or not included, or one of these two params can be set to 1. one_for_city and one_per_date query parameters work only on one-way requests. In case you want to create Return Trip itinerary calendar, you need to request Outbound and Inbound segments separately.
    /// </summary>
    public class OnePerDate : BaseNumericParam
    {
        public OnePerDate(long value) : base("one_per_date", value) { }
    }
}
