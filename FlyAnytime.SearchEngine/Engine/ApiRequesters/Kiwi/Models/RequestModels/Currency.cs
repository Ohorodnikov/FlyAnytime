using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// use this parameter to change the currency in the response
    /// </summary>
    public class Currency : BaseStringParam
    {
        public Currency(string value) : base("curr", value)
        {
        }
    }
}
