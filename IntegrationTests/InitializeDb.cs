using IntegrationTests.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using FlyAnytime.Tools;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class InitializeDb : HttpTestBase
    {
        [Fact]
        public void ReCreateDBs()
        {
            ResetDbs();
        }

        [Fact]
        public async Task SetTestData()
        {
            await FillData();
        }

        private async Task FillData()
        {
            CreateLocalizationLanguages();
            CreateCountries();
            CreateCities();
            CreateAirports();
        }

        private async Task<TModel> SendGetResult<TModel>(string request)
            where TModel : IBaseControllerModel, new()
        {
            var obj = new TModel();
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                //Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
                //Content = GetFormDataContent(obj),
            };

            dynamic data = await Send(msg, $"{obj.MicroserviceAlias}/{obj.ControllerName}/GetBy?{request}");

            if (data.success)
            {
                return JsonConvert.DeserializeObject<TModel>(JsonConvert.SerializeObject(data.data));
            }

            throw new Exception($"Cannot get {typeof(TModel)} for request {request}");
        }

        private async Task CreateLocalizationLanguages()
        {
            var eng = new Language
            {
                Code = "En",
                Name = "English",
                Culture = "en-US"
            };

            var ru = new Language
            {
                Code = "Ru",
                Name = "Русский",
                Culture = "ru-RU"
            };

            var ua = new Language
            {
                Code = "Ua",
                Name = "Українська",
                Culture = "uk-UA"
            };

            await SendCreateRequest(eng);
            await SendCreateRequest(ru);
            await SendCreateRequest(ua);
        }

        Language enLang;
        Language ruLang;
        Language uaLang;
        private async Task CreateCountries()
        {
            enLang = await SendGetResult<Language>("propName=Code&value=En");
            ruLang = await SendGetResult<Language>("propName=Code&value=Ru");
            uaLang = await SendGetResult<Language>("propName=Code&value=Ua");

            var usa = new Country
            {
                Code = "USA",
                Name = "United States",
                Localizations = new List<Localization>
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "США"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "USA"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "США"
                    },
                },
                CurrencyCode = "USD",
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
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Україна"
                    },
                },
                DefSearchCurrencyCode = "USD",
                CurrencyCode = "UAH"
            };

            await SendCreateRequest(usa);
            await SendCreateRequest(ua);
        }

        private async Task CreateCities()
        {
            var uaCountry = await SendGetResult<Country>("propName=Code&value=UA");
            var usaCountry = await SendGetResult<Country>("propName=Code&value=USA");

            var kyiv = new City
            {
                Code = "IEV",
                Name = "Kyiv",
                CountryId = uaCountry.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "Киев"
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
                        Value = "Одесса"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Odessa"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Одеса"
                    },
                }
            };
            
            var NY = new City
            {
                Code = "NYC",
                Name = "New York",
                CountryId = usaCountry.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "Нью-Йорк"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "New York"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Нью-Йорк"
                    },
                }
            };

            await SendCreateRequest(kyiv);
            await SendCreateRequest(odessa);
            await SendCreateRequest(NY);
        }

        private async Task CreateAirports()
        {
            var kyivCity = await SendGetResult<City>("propName=Code&value=IEV");

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
                        Value = "Жуляны"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Zhuliany"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Жуляни"
                    },
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
                        Value = "Аеропорт Борисполь"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Boryspil Airport"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Аеропорт Бориспіль"
                    },
                }
            };

            var nyCity = await SendGetResult<City>("propName=Code&value=NYC");
            var kennedy = new Airport
            {
                Code = "JFK",
                Name = "John F. Kennedy",
                CityId = nyCity.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "Международный аеропорт им. Джона Кеннеди"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "John F. Kennedy International Airport"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Міжнародний аеропорт ім. Джона Кенеді"
                    },
                }
            };
            var lga = new Airport
            {
                Code = "LGA",
                Name = "LaGuardia",
                CityId = nyCity.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "Аеропорт Ла Гвардия"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "LaGuardia Airport"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Аеропорт Ла Гвардія"
                    },
                }
            };
            var ewr = new Airport
            {
                Code = "EWR",
                Name = "Newark",
                CityId = nyCity.Id,
                Localizations = new List<Localization>()
                {
                    new Localization
                    {
                        LanguageId = ruLang.Id,
                        Value = "Международный аеропорт Неварк"
                    },
                    new Localization
                    {
                        LanguageId = enLang.Id,
                        Value = "Newark International Airport"
                    },
                    new Localization
                    {
                        LanguageId = uaLang.Id,
                        Value = "Міжнародний Аеропорт Неварк"
                    },
                }
            };

            await SendCreateRequest(zhuliany);
            await SendCreateRequest(boryspil);

            await SendCreateRequest(kennedy);
            await SendCreateRequest(lga);
            await SendCreateRequest(ewr);
        }

        private async Task TestGetAirport()
        {
            var kyivCity = await SendGetResult<Airport>("propName=Code&value=KBP");

            Assert.NotNull(kyivCity);
            Assert.NotEmpty(kyivCity.Id);
        }
    }
}
