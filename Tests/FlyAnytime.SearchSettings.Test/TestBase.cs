using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Test
{
    public abstract class TestBase
    {
        public string BaseUrl { get; set; } = "https://localhost:5021";
        public abstract string ControllerName { get; }

        public async Task<dynamic> Send(HttpRequestMessage msg, string route)
        {
            using (var http = new HttpClient())
            {
                var s = $"{BaseUrl}/{ControllerName}/{route}";
                if (string.IsNullOrEmpty(ControllerName))
                {
                    s = $"{BaseUrl}/{route}";
                }

                msg.RequestUri = new Uri(s);

                var res = await http.SendAsync(msg);

                return JsonConvert.DeserializeObject<ExpandoObject>(await res.Content.ReadAsStringAsync(), new ExpandoObjectConverter());
            }
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
    }
}
