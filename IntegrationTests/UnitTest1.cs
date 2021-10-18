using IntegrationTests.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IntegrationTests
{
    public class UnitTest1 : HttpTestBase
    {
        [Fact]
        public void InitBaseSetup()
        {
            ResetDbs();
            FillData();
        }

        private void ResetDbs()
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

        private void FillData()
        {
            CreateLocalizationLanguages();
            CreateCountries();
        }

        private dynamic SendCreateRequest<TData>(TData obj)
            where TData : IBaseControllerModel
        {
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
                //Content = GetFormDataContent(obj),
            };

            dynamic data = Send(msg, $"{obj.MicroserviceAlias}/{obj.ControllerName}/Create").GetAwaiter().GetResult(); ;

            //return data;
            if (data.success)
            {
                return data.data;
            }

            throw new Exception();
        }

        private TModel SendGetResult<TModel>(string request)
            where TModel : IBaseControllerModel, new()
        {
            var obj = new TModel();
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                //Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
                //Content = GetFormDataContent(obj),
            };

            dynamic data = Send(msg, $"{obj.MicroserviceAlias}/{obj.ControllerName}/GetBy?{request}").GetAwaiter().GetResult(); ;

            if (data.success)
            {
                return JsonConvert.DeserializeObject<TModel>(JsonConvert.SerializeObject(data.data));
            }

            throw new Exception();
        }

        private void CreateLocalizationLanguages()
        {
            var eng = new Language
            {
                Code = "En",
                Name = "English"
            };

            var ru = new Language
            {
                Code = "Ru",
                Name = "Русский"
            };

            SendCreateRequest(eng);
            SendCreateRequest(ru);
        }

        private void CreateCountries()
        {
            var enLang = SendGetResult<Language>("propName=Code&value=En");
            var ruLang = SendGetResult<Language>("propName=Code&value=Ru");

            var usa = new Country
            {
                Code = "USA",
                Name = "United States",
                Localizations = new List<Localization>
                {
                    new Localization
                    {
                        Id = "86C095BB-67A7-4D1C-B00A-0D4D304BC4FA",
                        LanguageId = ruLang.Id,
                        Value = "США"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "United States of America"
                    }
                }
            };

            var ua = new Country
            {
                Code = "UA",
                Name = "Ukraine",
                Localizations = new List<Localization>
                {
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Ukraine"
                    },
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "Украина"
                    }
                }
            };

            SendCreateRequest(usa);
            SendCreateRequest(ua);
        }
    }
}
