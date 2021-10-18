using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Test
{
    public class Language
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class LanguageTest : TestBase
    {
        public override string ControllerName => "Language";

        public async Task<string> CreateLanguage(object obj)
        {
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = GetFormDataContent(obj),
            };

            dynamic data = await Send(msg, "Create");

            if (data.success)
            {
                return data.data.savedId;
            }

            throw new Exception();
        }

        public async Task<Language> GetById(string id)
        {
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
            };

            dynamic data = await Send(msg, $"Get?id={id}");

            if (data.success)
            {
                return JsonConvert.DeserializeObject<Language>(JsonConvert.SerializeObject(data.data));
            }
            throw new Exception();
        }

        public async Task<(int total, IEnumerable<Language> items)> GetMany(int page, int pageSize)
        {
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
            };

            dynamic data = await Send(msg, $"GetMany?page={page}&pageSize={pageSize}");

            if (data.success)
            {
                var total = int.Parse(data.data.total.ToString());

                var items = JsonConvert.DeserializeObject<IEnumerable<Language>>(JsonConvert.SerializeObject(data.data.items));

                return (total, items);
            }
            throw new Exception();
        }
    }
}
