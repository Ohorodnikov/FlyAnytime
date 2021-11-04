namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    public class FlightType : BaseStringParam
    {
        /// <summary>
        /// switch for oneway/round flights search - will be deprecated in the near future (until then, you have to use the round parameter if one from the nights_in_dst of return date parameters is given.)
        /// </summary>
        /// <param name="value"></param>
        public FlightType(string value) : base("flight_type", value)
        {
        }
    }
}
