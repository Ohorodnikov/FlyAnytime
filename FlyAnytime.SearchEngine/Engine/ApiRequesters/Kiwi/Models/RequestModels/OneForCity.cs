namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// It returns the cheapest flights to every city covered by the to parameter. E.g. if you set it to 1 and your search is from PRG to LON/BUD/NYC, you'll get 3 results: the cheapest PRG-LON, the cheapest PRG-BUD, and the cheapest PRG-NYC. one_for_city and one_per_date query parameters work only on one-way requests. In case you want to create Return Trip itinerary calendar, you need to request Outbound and Inbound segments separately.
    /// </summary>
    public class OneForCity : BaseNumericParam
    {
        public OneForCity(long value) : base("one_for_city", value) { }
    }
}
