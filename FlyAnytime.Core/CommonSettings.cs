using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core
{
    /// <summary>
    /// Register as singletone
    /// </summary>
    public interface ICommonSettings
    {
        string ApiGatewayUrl { get; set; }
    }

    public class CommonSettings : ICommonSettings
    {
        public string ApiGatewayUrl { get; set; }
    }
}
