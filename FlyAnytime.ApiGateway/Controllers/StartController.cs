using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlyAnytime.ApiGateway.Controllers
{
    public class StartController : Controller
    {
        [Route("start")]
        public async Task<IActionResult> Start([FromServices] IConfiguration configuration)
        {
            var ngrokUrl = configuration.GetSection("SelfUrl").Value;
            var routeMaps = configuration.GetSection("RouteMap").Get<RouteMap[]>();

            var initResult = new List<object>();

            var http = new HttpClient();
            foreach (var routeMap in routeMaps)
            {
                var url = $"{ngrokUrl}{routeMap.Route}/init";
                var res = await http.GetAsync(url);

                initResult.Add(new { success = res.IsSuccessStatusCode, url = url });
            }

            return Json(initResult);
        }
    }
}
