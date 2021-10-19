using IntegrationTests.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using FlyAnytime.Tools;

namespace IntegrationTests
{
    public class InitializeDb : HttpTestBase
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
            CreateCities();
            CreateAirports();
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

            throw new Exception($"Cannot get {typeof(TModel)} for request {request}");
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
                Name = "�������"
            };

            SendCreateRequest(eng);
            SendCreateRequest(ru);
        }

        Language enLang;
        Language ruLang;
        private void CreateCountries()
        {
            enLang = SendGetResult<Language>("propName=Code&value=En");
            ruLang = SendGetResult<Language>("propName=Code&value=Ru");

            var usa = new Country
            {
                Code = "USA",
                Name = "United States",
                Localizations = new List<Localization>
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "���"
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
                        Value = "�������"
                    }
                }
            };

            SendCreateRequest(usa);
            SendCreateRequest(ua);
        }

        private void CreateCities()
        {
            var uaCountry = SendGetResult<Country>("propName=Code&value=UA");
            var usaCountry = SendGetResult<Country>("propName=Code&value=USA");

            var kyiv = new City
            {
                Code = "Kyiv",
                Name = "Kyiv",
                CountryId = uaCountry.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "����"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Kyiv"
                    }
                }
            };

            var odessa = new City
            {
                Code = "Odessa",
                Name = "Odessa",
                CountryId = uaCountry.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "������"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Odessa"
                    }
                }
            };
            
            var NY = new City
            {
                Code = "NY",
                Name = "New York",
                CountryId = usaCountry.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "���-����"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "New York"
                    }
                }
            };

            SendCreateRequest(kyiv);
            SendCreateRequest(odessa);
            SendCreateRequest(NY);
        }

        private void CreateAirports()
        {
            var kyivCity = SendGetResult<City>("propName=Code&value=Kyiv");

            var zhuliany = new Airport
            {
                Code = "IEV",
                Name = "Zhuliany",
                CityId = kyivCity.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "������"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Zhuliany"
                    }
                }
            };
            var boryspil = new Airport
            {
                Code = "KBP",
                Name = "Boryspil",
                CityId = kyivCity.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "�������� ���������"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Boryspil Airport"
                    }
                }
            };

            SendCreateRequest(zhuliany);
            SendCreateRequest(boryspil);
        }

        private void TestGetAirport()
        {
            var kyivCity = SendGetResult<Airport>("propName=Code&value=KBP");

            Assert.NotNull(kyivCity);
            Assert.NotEmpty(kyivCity.Id);
        }
    }
}
