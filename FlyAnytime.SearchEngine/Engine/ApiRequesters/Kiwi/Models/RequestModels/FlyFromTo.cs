using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// Kiwi API ID of the departure location. It accepts multiple values separated by a comma, these values might be airport codes, city IDs, two letter country codes, metropolitan codes and radiuses as well as a subdivision, region, autonomous_territory, continent and specials (Points of interest, such as Times Square).
    ///Some locations have the same code for airport and metropolis(city), e.g.DUS stands for metro code Duesseldorf, Moenchengladbach and Weeze as well as Duesseldorf airport.See the following examples:
    ///'fly_from=city:DUS' will match all airports in "DUS", "MGL" and "NRN" (all in the city of Duesseldorf)
    ///'fly_from=DUSf will do the same as the above
    ///'fly_from=airport:DUS' will only match airport "DUS"
    ///Radius needs to be in form lat-lon-xkm.The number of decimal places for radius is limited to 6. E.g.-23.24--47.86-500km for places around Sao Paulo. 'LON' - checks every airport in London, 'LHR' - checks flights from London Heathrow, 'UK' - flights from the United Kingdom.Link to Locations API.
    /// </summary>
    public class FlyFrom : BaseStringParam
    {
        public FlyFrom(string value) : base("fly_from", value) { }
        public override string ToSearchString() => $"city:{ParamValue}";
    }

    /// <summary>
    /// Kiwi api ID of the arrival destination. It accepts the same values in the same format as the 'fly_from' parameter
    ///If you don’t include any value you’ll get aggregated results for some airports.
    /// </summary>
    public class FlyTo : BaseStringParam
    {
        public FlyTo(string value) : base("fly_to", value) { }
        public override string ToSearchString() => $"city:{ParamValue}";
    }

    public static class FlyFromToExt
    {
        public static IRequestDescriptor SetFlyDirection(this IRequestDescriptor request, string cityFrom, string cityTo)
        {
            request
                .AddParam(new FlyFrom(cityFrom))
                .AddParam(new FlyTo(cityTo));

            return request;
        }
    }
}
