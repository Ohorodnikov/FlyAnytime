using IntegrationTests.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class HttpTestBase
    {
        private class GatewayResult
        {
            public bool Success { get; set; }
            public string Content { get; set; }
        }

        public string BaseUrl { get; set; } = "https://localhost:5001";//ApiGatewayUrl
        //public abstract string ControllerName { get; }
        public async Task<dynamic> Send(HttpRequestMessage msg, string route)
        {
            GatewayResult r;
            using (var http = new HttpClient())
            {
                var s = $"{BaseUrl}/{route}";

                msg.RequestUri = new Uri(s);

                var res = await http.SendAsync(msg);
                var content = await res.Content.ReadAsStringAsync();
                dynamic q = JsonConvert.DeserializeObject(content);
                var t = q.Type;
                if (t != null && ((JTokenType)t) == JTokenType.Array)
                {
                    var list = new List<object>();
                    var l = q.GetEnumerator();
                    while (l.MoveNext())
                    {
                        var str = (string)l.Current.ToString();
                        var dynamic = JsonConvert.DeserializeObject<ExpandoObject>(str, new ExpandoObjectConverter());
                        list.Add(dynamic);
                    }

                    return list;
                }
                r = JsonConvert.DeserializeObject<GatewayResult>(content);
            }

            if (r.Success)
            {
                if (r.Content.Trim().StartsWith("{"))
                    return JsonConvert.DeserializeObject<ExpandoObject>(r.Content, new ExpandoObjectConverter());
                else
                    return r.Content;
            }

            throw new Exception(r.Content);
        }

        protected MultipartFormDataContent GetFormDataContent(object ent)
        {
            var t = ent.GetType();
            var pr = t.GetProperties();

            var content = new MultipartFormDataContent();

            foreach (var p in pr)
            {
                var v = p.GetValue(ent);
                if (v == null)
                    continue;

                content.Add(new StringContent(v.ToString()), p.Name);
            }

            return content;
        }

        protected async Task<dynamic> SendCreateRequest<TData>(TData obj)
            where TData : IBaseControllerModel
        {
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
                //Content = GetFormDataContent(obj),
            };

            dynamic data = await Send(msg, $"{obj.MicroserviceAlias}/{obj.ControllerName}/Create");

            //return data;
            if (data.success)
            {
                return data.data;
            }

            throw new Exception();
        }

        protected void ResetDbs()
        {
            var key = "19D0DE6E-77A0-4F0D-A9AC-65AEA1470BCB";

            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
            };

            msg.Headers.Add("resetDbSecretKey", key);
            msg.Headers.Add("Referer", "SearchSettings.Test");

            var r = Send(msg, "ReCreateAllDb").GetAwaiter().GetResult();
        }
    }
}
