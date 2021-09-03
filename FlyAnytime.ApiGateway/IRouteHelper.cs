﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.ApiGateway
{
    public interface IRouteHelper
    {
        RouteMap[] RouteMaps { get; }
        Task RouteRequest(HttpContext context);
    }

    public class RouteHelper : IRouteHelper
    {
        public RouteHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            RouteMaps = Configuration.GetSection("RouteMap").Get<RouteMap[]>();

        }
        static HttpClient client = new HttpClient();

        public IConfiguration Configuration { get; }

        public RouteMap[] RouteMaps { get; }

        public async Task RouteRequest(HttpContext context)
        {
            await context.Response.WriteAsync(await GetResultFromRedirect(context.Request));
        }

        private async Task<string> GetResultFromRedirect(HttpRequest request)
        {
            var resultContent = string.Empty;
            if (!TryFindRedirectRoute(request, out var redirect))
            {
                return resultContent;
            }

            if (redirect.NeedAuth)
            {
                var authUrl = Configuration.GetSection("AuthRoute").Value;
                using (var newRequest = new HttpRequestMessage(new HttpMethod(request.Method), authUrl))
                {
                    if (request.Headers.TryGetValue("Authorization", out var res))
                    {
                        newRequest.Headers.Add("Authorization", res.ToList());
                    }
                    var authResponse = await client.SendAsync(newRequest);
                    if (!authResponse.IsSuccessStatusCode)
                        return GetRedirectResult("Not auth", false);
                    var authResponseStr = await authResponse.Content.ReadAsStringAsync();
                    var q = (dynamic)JsonConvert.DeserializeObject(authResponseStr);
                    if (q.jwtStatus == false)
                        return GetRedirectResult("Not auth", false);
                }
            }

            var redirectUrl = CreateRedirectUrl(request, redirect);

            var redirectResult = await RedirectTo(request, redirectUrl);

            var msg = redirectResult.IsSuccessStatusCode ? await redirectResult.Content.ReadAsStringAsync() : redirectResult.ReasonPhrase;

            return GetRedirectResult(msg, redirectResult.IsSuccessStatusCode);
        }

        private string GetRedirectResult(string redirectResponse, bool success)
        {
            var res = new 
            { 
                success = success,
                content = redirectResponse
            };

            return JsonConvert.SerializeObject(res);

        }

        private bool TryFindRedirectRoute(HttpRequest request, out RouteMap redirect)
        {
            redirect = null;
            if (!request.Path.HasValue)
                return false;

            var startPath = request.Path.ToUriComponent().Split("/", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (startPath == null)
            {
                return false;
            }

            startPath = "/" + startPath;

            redirect = RouteMaps.FirstOrDefault(x => x.Route.Equals(startPath, StringComparison.InvariantCultureIgnoreCase));

            return redirect != null;
        }

        private string CreateRedirectUrl(HttpRequest request, RouteMap routeMap)
        {
            var reqPath = request.Path.ToUriComponent();
            var queryParams = request.QueryString.ToString();
            var redirectBase = routeMap.RedirectTo;

            var pathParts = reqPath.Split("/", StringSplitOptions.RemoveEmptyEntries).Skip(1);
            var redirectPath = "/" + string.Join("/", pathParts);

            return redirectBase + redirectPath + queryParams;
        }

        private async Task<HttpResponseMessage> RedirectTo(HttpRequest request, string redirectUrl)
        {
            string requestContent;
            using (var receiveStream = request.Body)
            {
                using var readStream = new StreamReader(receiveStream, Encoding.UTF8);
                requestContent = await readStream.ReadToEndAsync();
            }

            using (var newRequest = new HttpRequestMessage(new HttpMethod(request.Method), redirectUrl))
            {
                newRequest.Content = new StringContent(requestContent, Encoding.UTF8, request.ContentType);
                if (request.Headers.TryGetValue("Authorization", out var res))
                {
                    newRequest.Headers.Add("Authorization", res.ToList());
                }
                return await client.SendAsync(newRequest);
            }
        }
    }
}