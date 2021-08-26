using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.ApiGateway
{
    public class RouteMap
    {
        public string Route { get; set; }
        public string RedirectTo { get; set; }
        public bool NeedAuth { get; set; }
    }
}
