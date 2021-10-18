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
        IConfiguration _configuration;
        public StartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<List<object>> SendWOAuth(string route, HttpMethod method, Dictionary<string, string> headers = null)
        {
            headers = headers ?? new Dictionary<string, string>();
            var ngrokUrl = _configuration.GetSection("SelfUrlLocal").Value;
            var routeMaps = _configuration.GetSection("RouteMap").Get<RouteMap[]>();

            var initResult = new List<object>();

            var http = new HttpClient();
            foreach (var routeMap in routeMaps)
            {
                var url = $"{ngrokUrl}{routeMap.Route}/{route}";
                using (var newRequest = new HttpRequestMessage(method, url))
                {
                    foreach (var header in headers)
                    {
                        newRequest.Headers.Add(header.Key, header.Value);
                    }

                    newRequest.Headers.Add("SkipAuth", "2BD696F0-8E4A-416F-AB17-786BDB10D044");

                    var res = await http.SendAsync(newRequest);
                    var msg = await res.Content.ReadAsStringAsync();

                    initResult.Add(new { success = res.IsSuccessStatusCode, url = url, data = msg });
                }
            }

            return initResult;
        }

        [Route("start")]
        public async Task<IActionResult> Start()
        {
            var initResult = await SendWOAuth("Init", HttpMethod.Get);

            //var ngrokUrl = configuration.GetSection("SelfUrlLocal").Value;
            //var routeMaps = configuration.GetSection("RouteMap").Get<RouteMap[]>();

            //var initResult = new List<object>();

            //var http = new HttpClient();
            //foreach (var routeMap in routeMaps)
            //{
            //    var url = $"{ngrokUrl}{routeMap.Route}/init";
            //    var res = await http.GetAsync(url);

            //    initResult.Add(new { success = res.IsSuccessStatusCode, url = url });
            //}

            return Json(initResult);
        }

        [HttpPut]
        [Route("ReCreateAllDb")]
        public async Task<IActionResult> ReCreateAllDb()
        {
            var secretKey = Request.Headers["resetDbSecretKey"].ToString();
            var referer = Request.Headers["Referer"].ToString();

            if (secretKey != "19D0DE6E-77A0-4F0D-A9AC-65AEA1470BCB" || referer != "SearchSettings.Test")
            {
                return NotFound();
            }

            var headers = new Dictionary<string, string>
            {
                {"resetDbSecretKey", secretKey},
                {"Referer", referer},
            };

            var initResult = await SendWOAuth("ReCreateDb", HttpMethod.Post, headers);

            //var ngrokUrl = configuration.GetSection("SelfUrlLocal").Value;
            //var routeMaps = configuration.GetSection("RouteMap").Get<RouteMap[]>();

            //var initResult = new List<object>();

            //var http = new HttpClient();
            //foreach (var routeMap in routeMaps)
            //{
            //    var url = $"{ngrokUrl}{routeMap.Route}/ReCreateDb";
            //    using (var newRequest = new HttpRequestMessage(HttpMethod.Post, url))
            //    {
            //        newRequest.Headers.Add("resetDbSecretKey", secretKey);
            //        newRequest.Headers.Add("Referer", referer);
                    
            //        var res = await http.SendAsync(newRequest);
            //        var msg = res.Content.ReadAsStringAsync().Result;

            //        initResult.Add(new { success = res.IsSuccessStatusCode, url = url, data = msg });
            //    }
            //}

            return Json(initResult);
        }
    }
}
