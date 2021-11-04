using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// result filter, minimal price
    /// </summary>
    public class PriceFrom : BaseNumericParam
    {
        public PriceFrom(long value) : base("price_from", value) { }
    }

    /// <summary>
    /// result filter, maximal price
    /// </summary>
    public class PriceTo : BaseNumericParam
    {
        public PriceTo(long value) : base("price_to", value) { }
    }
}
