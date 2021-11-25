using FlyAnytime.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.Helpers
{
    public static class UrlHelperExt
    {
        public static string CreateAbsoluteUrl<TController>(this LinkGenerator helper, 
            Expression<Func<TController, object>> action, object data, HostInfo hostInfo)
            where TController : ControllerBase
        {
            var methodName = action.GetStringBody().Split('(')[0];
            var controllerName = typeof(TController).Name.Replace("Controller", "");
            var hostStr = new HostString(hostInfo.Name, hostInfo.Port);

            return helper.GetUriByAction(methodName, controllerName, data, "https", hostStr);
        }
    }
}
