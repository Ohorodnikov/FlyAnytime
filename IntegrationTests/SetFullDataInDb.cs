using FlyAnytime.Tools;
using IntegrationTests.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class SetFullDataInDb : HttpTestBase
    {
        private async Task<IEnumerable<TModel>> SendGetManyResult<TModel>(int page, int pageSize)
            where TModel : IBaseControllerModel, new()
        {
            var obj = new TModel();
            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
            };

            dynamic data = await Send(msg, $"{obj.MicroserviceAlias}/{obj.ControllerName}/GeMany?page={page}&pageSize={pageSize}");

            if (data.success)
            {
                return JsonConvert.DeserializeObject<IEnumerable<TModel>>(JsonConvert.SerializeObject(data.data));
            }

            throw new Exception($"Cannot get {typeof(TModel)} for request");
        }

        [Fact]
        public async Task FillCountriesJSON()
        {
            var en = new Language
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

            var de = new Language
            {
                Code = "De",
                Name = "Deutsch",
                Culture = "de-DE"
            };

            var es = new Language
            {
                Code = "Es",
                Name = "Español",
                Culture = " es-ES"
            };

            var fr = new Language
            {
                Code = "Fr",
                Name = "Français",
                Culture = "fr-FR"
            };

            var pt = new Language
            {
                Code = "Pt",
                Name = "Português",
                Culture = "pt-PT"
            };

            var langs = new List<Language> { en, ru, ua, de, es, fr, pt };

            var countries = await GetData2Save(langs);

            CheckCountries(countries);
        }

        [Fact]
        public async Task FillDbByAllValues()
        {
            var en = new Language
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

            var de = new Language
            {
                Code = "De",
                Name = "Deutsch",
                Culture = "de-DE"
            };

            var es = new Language
            {
                Code = "Es",
                Name = "Español",
                Culture = " es-ES"
            };

            var fr = new Language
            {
                Code = "Fr",
                Name = "Français",
                Culture = "fr-FR"
            };

            var pt = new Language
            {
                Code = "Pt",
                Name = "Português",
                Culture = "pt-PT"
            };

            var langs = new List<Language> { en, ru, ua, de, es, fr, pt };

            foreach (var l in langs)
            {
                var saveRes = await SendCreateRequest(l);

                l.Id = saveRes.savedId;
            }

            var allCountries = await GetData2Save(langs);

            SetLangIds(allCountries, langs);

            CheckCountries(allCountries);

            await DoInParallel(allCountries, SaveCountry);

            var allCities = new List<City2Airp>(3300);
            foreach (var c in allCountries)
            {
                foreach (var cit in c.Cities)
                {
                    cit.City.CountryId = c.Country.Id;
                    if (!allCities.Any(x => x.City.Code == cit.City.Code))
                        allCities.Add(cit);
                }
            }

            await DoInParallel(allCities, SaveCity);           

            var allAirps = new List<Airport>(3300);
            foreach (var c in allCities)
            {
                foreach (var air in c.Airports)
                {
                    air.CityId = c.City.Id;
                    if (!allAirps.Any(x => x.Code == air.Code))
                        allAirps.Add(air);
                }
            }

            await DoInParallel(allAirps, SaveAirport);
        }

        private async Task DoInParallel<T>(List<T> ts, Func<T, Task> func)
        {
            var batchSize = 40;
            var current = 0;

            while (current <= ts.Count)
            {
                var ts2Save = ts.Skip(current).Take(batchSize).ToList();
                current += batchSize;

                var tasks = new List<Task>(batchSize);
                foreach (var el in ts2Save)
                    tasks.Add(func(el));

                await Task.WhenAll(tasks);
            }
        }

        private async Task SaveCountry(CountryWithCity country)
        {
            var countrySaveRes = await SendCreateRequest(country.Country);
            var countId = countrySaveRes.savedId;

            country.Country.Id = countId;
        }

        private async Task SaveCity(City2Airp city)
        {
            var citySaveRes = await SendCreateRequest(city.City);
            var cityId = citySaveRes.savedId;

            city.City.Id = cityId;
        }

        private async Task SaveAirport(Airport air)
        {
            await SendCreateRequest(air);
        }

        private void SetLangIds(List<CountryWithCity> countries, List<Language> languages)
        {
            var code2id = languages.ToDictionary(l => l.Code, l => l.Id);

            void SetIds(IEnumerable<ILocalizationEntity> entities)
            {
                foreach (var e in entities)
                    foreach (var lang in e.Localizations)
                        lang.LanguageId = code2id[lang.LanguageCode];
            }

            SetIds(countries.Select(x => x.Country));
            SetIds(countries.SelectMany(x => x.Cities).Select(x => x.City));
            SetIds(countries.SelectMany(x => x.Cities).SelectMany(x => x.Airports));
        }

        private void CheckCountries(List<CountryWithCity> countries)
        {
            foreach (var country in countries)
            {
                if (country.Country.Code.IsNullOrEmpty())
                    throw new Exception($"Country {country.Country.Name} has no code");

                foreach (var city in country.Cities)
                {
                    if (city.City.Code.IsNullOrEmpty())
                        throw new Exception($"City {city.City.Name} has no code in country {country.Country.Code}");

                    foreach (var airp in city.Airports)
                    {
                        if (airp.Code.IsNullOrEmpty())
                            throw new Exception($"Airport {airp.Name} has no code in city {city.City.Code} in country {country.Country.Code}");
                    }
                }
            }
        }

        [DebuggerDisplay("{Country.Code}")]
        private class CountryWithCity
        {
            public Country Country { get; set; }
            public List<City2Airp> Cities { get; set; } = new List<City2Airp>();
        }

        private async Task<List<CountryWithCity>> GetData2Save(List<Language> languages)
        {
            var folder = new DirectoryInfo(Directory.GetCurrentDirectory())
                .Parent //Folder Debug
                .Parent //Folder bin
                .Parent //Folder IntegrationTests
                .Parent //Folder with *.sln
                .FullName;

            var path = folder + "\\countries.json";
            var res = new List<CountryWithCity>(260);
            if (File.Exists(path))
            {
                var dataStr = File.ReadAllText(path);

                var data = JsonConvert.DeserializeObject<List<CountryWithCity>>(dataStr);

                res = data.Where(x => x.Country != null).ToList();

                if (data != null && data.Count > 210)
                    return data;
            }

            var maxRequest = 98;
            var batchSize = maxRequest/languages.Count;

            var cc = res.Select(x => x.Country.Code).ToList();

            var countryCodes = GetCountryCodeNameCurrencies()
                .Where(x => !cc.Contains(x.CountryCode))
                .ToList();

            var current = 0;
            while (current <= countryCodes.Count)
            {
                var codes2request = countryCodes.Skip(current).Take(batchSize).ToList();
                current += batchSize;
                var tasks = new List<Task<CountryWithCity>>(batchSize);

                var sw = new Stopwatch();
                sw.Start();
                foreach (var code in codes2request)
                {
                    tasks.Add(GetOneCountryInfo(code, languages));
                }

                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    res.Add(await task);
                }
                sw.Stop();
                var minute = 1000 * 60;
                await Task.Delay((minute - (int)sw.ElapsedMilliseconds) + 2000);
            }

            var resultList = new List<CountryWithCity>(res.Count);

            foreach (var re in res.Where(x => x.Country != null))
            {
                var r = resultList.FirstOrDefault(x => x.Country.Code == re.Country.Code);

                if (r == null)
                    resultList.Add(re);
                else
                    r.Cities.AddRange(re.Cities);
            }

            var json = JsonConvert.SerializeObject(resultList);

            await File.WriteAllTextAsync(path, json);

            return res;
        }

        private class City2Airp
        {
            public City City { get; set; }
            public List<Airport> Airports { get; set; } = new List<Airport>();
        }

        private async Task<CountryWithCity> GetOneCountryInfo(CountryCodeNameCurrency countryCodeInfo, List<Language> languages)
        {
            var httpClient = new HttpClient();
            var server = "https://tequila-api.kiwi.com/locations/subentity";
            httpClient.DefaultRequestHeaders.Add("apikey", "EOWiRQTxLIwmfRxTquwvGY1Ql6I0vrB3");

            Country country = null;

            var cities = new List<City2Airp>();

            foreach (var lan in languages)
            {
                var res = await httpClient.GetAsync($"{server}?term={countryCodeInfo.CountryCode}&locale={lan.Culture}&location_types=airport&limit=10000&sort=name&active_only=true");

                if (!res.IsSuccessStatusCode)
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        continue;
                    }
                    else if (res.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                var dataStr = await res.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<Airports>(dataStr);

                if (data.ResultsRetrieved == 0)
                    continue;

                var resCountry = data.Locations[0].City.Country;

                if (country == null)
                {
                    country = new Country
                    {
                        Code = resCountry.Code,
                        CurrencyCode = countryCodeInfo.CurrCode,
                        Localizations = new List<Localization>()
                    };
                }

                AddLocalizationValue(country, lan, resCountry.Name);

                foreach (var airpRes in data.Locations)
                {
                    var city = FindOrCreateCity(cities, airpRes.City.Code ?? airpRes.Code);
                    AddLocalizationValue(city.City, lan, airpRes.City.Name);

                    var airp = FindOrCreateByCode(city.Airports, airpRes.Code);

                    AddLocalizationValue(airp, lan, airpRes.Name);                    
                }
            }

            return new CountryWithCity 
            { 
                Country = country,
                Cities = cities
            };
        }

        private void CheckLocalization<TEnt>(TEnt ent)
            where TEnt : ILocalizationEntity
        {
            Assert.True(ent.Localizations.Count == 7, $"{ent.Code} don't have all 7 localizations");
        }

        private void AddLocalizationValue<TEnt>(TEnt entity, Language lang, string value)
            where TEnt : ILocalizationEntity
        {
            var loc = new Localization
            {
                LanguageCode = lang.Code,
                Value = value
            };

            if (entity.Localizations.Any(x => x.LanguageCode == loc.LanguageCode))
                return;

            entity.Localizations.Add(loc);

            if (lang.Code == "En")
                entity.Name = loc.Value;
        }

        private City2Airp FindOrCreateCity(List<City2Airp> map, string code)
        {
            var res = map.FirstOrDefault(x => x.City.Code == code);

            if (res != null)
                return res;

            var item = new City2Airp
            {
                City = new City
                {
                    Code = code,
                    Localizations = new List<Localization>()
                }
            };

            map.Add(item);

            return item;
        }

        private ILocalizationEntity FindOrCreateByCode<TEnt>(List<TEnt> searchList, string code)
            where TEnt : ILocalizationEntity, new()
        {
            var res = searchList.FirstOrDefault(x => x.Code == code);

            if (res != null)
                return res;

            var item = new TEnt
            {
                Code = code,
                Localizations = new List<Localization>()
            };

            searchList.Add(item);

            return item;
        }

        private List<Code2Name> AllCountryCode2Name()
        {
            var str =
                "[{\"Code\": \"AF\", \"Name\": \"Afghanistan\"},{\"Code\": \"AX\", \"Name\": \"\u00c5land Islands\"},{\"Code\": \"AL\", \"Name\": \"Albania\"},{\"Code\": \"DZ\", \"Name\": \"Algeria\"},{\"Code\": \"AS\", \"Name\": \"American Samoa\"},{\"Code\": \"AD\", \"Name\": \"Andorra\"},{\"Code\": \"AO\", \"Name\": \"Angola\"},{\"Code\": \"AI\", \"Name\": \"Anguilla\"},{\"Code\": \"AQ\", \"Name\": \"Antarctica\"},{\"Code\": \"AG\", \"Name\": \"Antigua and Barbuda\"},{\"Code\": \"AR\", \"Name\": \"Argentina\"},{\"Code\": \"AM\", \"Name\": \"Armenia\"},{\"Code\": \"AW\", \"Name\": \"Aruba\"},{\"Code\": \"AU\", \"Name\": \"Australia\"},{\"Code\": \"AT\", \"Name\": \"Austria\"},{\"Code\": \"AZ\", \"Name\": \"Azerbaijan\"},{\"Code\": \"BS\", \"Name\": \"Bahamas\"},{\"Code\": \"BH\", \"Name\": \"Bahrain\"},{\"Code\": \"BD\", \"Name\": \"Bangladesh\"},{\"Code\": \"BB\", \"Name\": \"Barbados\"},{\"Code\": \"BY\", \"Name\": \"Belarus\"},{\"Code\": \"BE\", \"Name\": \"Belgium\"},{\"Code\": \"BZ\", \"Name\": \"Belize\"},{\"Code\": \"BJ\", \"Name\": \"Benin\"},{\"Code\": \"BM\", \"Name\": \"Bermuda\"},{\"Code\": \"BT\", \"Name\": \"Bhutan\"},{\"Code\": \"BO\", \"Name\": \"Bolivia, Plurinational State of\"},{\"Code\": \"BQ\", \"Name\": \"Bonaire, Sint Eustatius and Saba\"},{\"Code\": \"BA\", \"Name\": \"Bosnia and Herzegovina\"},{\"Code\": \"BW\", \"Name\": \"Botswana\"},{\"Code\": \"BV\", \"Name\": \"Bouvet Island\"},{\"Code\": \"BR\", \"Name\": \"Brazil\"},{\"Code\": \"IO\", \"Name\": \"British Indian Ocean Territory\"},{\"Code\": \"BN\", \"Name\": \"Brunei Darussalam\"},{\"Code\": \"BG\", \"Name\": \"Bulgaria\"},{\"Code\": \"BF\", \"Name\": \"Burkina Faso\"},{\"Code\": \"BI\", \"Name\": \"Burundi\"},{\"Code\": \"KH\", \"Name\": \"Cambodia\"},{\"Code\": \"CM\", \"Name\": \"Cameroon\"},{\"Code\": \"CA\", \"Name\": \"Canada\"},{\"Code\": \"CV\", \"Name\": \"Cape Verde\"},{\"Code\": \"KY\", \"Name\": \"Cayman Islands\"},{\"Code\": \"CF\", \"Name\": \"Central African Republic\"},{\"Code\": \"TD\", \"Name\": \"Chad\"},{\"Code\": \"CL\", \"Name\": \"Chile\"},{\"Code\": \"CN\", \"Name\": \"China\"},{\"Code\": \"CX\", \"Name\": \"Christmas Island\"},{\"Code\": \"CC\", \"Name\": \"Cocos (Keeling) Islands\"},{\"Code\": \"CO\", \"Name\": \"Colombia\"},{\"Code\": \"KM\", \"Name\": \"Comoros\"},{\"Code\": \"CG\", \"Name\": \"Congo\"},{\"Code\": \"CD\", \"Name\": \"Congo, the Democratic Republic of the\"},{\"Code\": \"CK\", \"Name\": \"Cook Islands\"},{\"Code\": \"CR\", \"Name\": \"Costa Rica\"},{\"Code\": \"CI\", \"Name\": \"C\u00f4te d'Ivoire\"},{\"Code\": \"HR\", \"Name\": \"Croatia\"},{\"Code\": \"CU\", \"Name\": \"Cuba\"},{\"Code\": \"CW\", \"Name\": \"Cura\u00e7ao\"},{\"Code\": \"CY\", \"Name\": \"Cyprus\"},{\"Code\": \"CZ\", \"Name\": \"Czech Republic\"},{\"Code\": \"DK\", \"Name\": \"Denmark\"},{\"Code\": \"DJ\", \"Name\": \"Djibouti\"},{\"Code\": \"DM\", \"Name\": \"Dominica\"},{\"Code\": \"DO\", \"Name\": \"Dominican Republic\"},{\"Code\": \"EC\", \"Name\": \"Ecuador\"},{\"Code\": \"EG\", \"Name\": \"Egypt\"},{\"Code\": \"SV\", \"Name\": \"El Salvador\"},{\"Code\": \"GQ\", \"Name\": \"Equatorial Guinea\"},{\"Code\": \"ER\", \"Name\": \"Eritrea\"},{\"Code\": \"EE\", \"Name\": \"Estonia\"},{\"Code\": \"ET\", \"Name\": \"Ethiopia\"},{\"Code\": \"FK\", \"Name\": \"Falkland Islands (Malvinas)\"},{\"Code\": \"FO\", \"Name\": \"Faroe Islands\"},{\"Code\": \"FJ\", \"Name\": \"Fiji\"},{\"Code\": \"FI\", \"Name\": \"Finland\"},{\"Code\": \"FR\", \"Name\": \"France\"},{\"Code\": \"GF\", \"Name\": \"French Guiana\"},{\"Code\": \"PF\", \"Name\": \"French Polynesia\"},{\"Code\": \"TF\", \"Name\": \"French Southern Territories\"},{\"Code\": \"GA\", \"Name\": \"Gabon\"},{\"Code\": \"GM\", \"Name\": \"Gambia\"},{\"Code\": \"GE\", \"Name\": \"Georgia\"},{\"Code\": \"DE\", \"Name\": \"Germany\"},{\"Code\": \"GH\", \"Name\": \"Ghana\"},{\"Code\": \"GI\", \"Name\": \"Gibraltar\"},{\"Code\": \"GR\", \"Name\": \"Greece\"},{\"Code\": \"GL\", \"Name\": \"Greenland\"},{\"Code\": \"GD\", \"Name\": \"Grenada\"},{\"Code\": \"GP\", \"Name\": \"Guadeloupe\"},{\"Code\": \"GU\", \"Name\": \"Guam\"},{\"Code\": \"GT\", \"Name\": \"Guatemala\"},{\"Code\": \"GG\", \"Name\": \"Guernsey\"},{\"Code\": \"GN\", \"Name\": \"Guinea\"},{\"Code\": \"GW\", \"Name\": \"Guinea-Bissau\"},{\"Code\": \"GY\", \"Name\": \"Guyana\"},{\"Code\": \"HT\", \"Name\": \"Haiti\"},{\"Code\": \"HM\", \"Name\": \"Heard Island and McDonald Islands\"},{\"Code\": \"VA\", \"Name\": \"Holy See (Vatican City State)\"},{\"Code\": \"HN\", \"Name\": \"Honduras\"},{\"Code\": \"HK\", \"Name\": \"Hong Kong\"},{\"Code\": \"HU\", \"Name\": \"Hungary\"},{\"Code\": \"IS\", \"Name\": \"Iceland\"},{\"Code\": \"IN\", \"Name\": \"India\"},{\"Code\": \"ID\", \"Name\": \"Indonesia\"},{\"Code\": \"IR\", \"Name\": \"Iran, Islamic Republic of\"},{\"Code\": \"IQ\", \"Name\": \"Iraq\"},{\"Code\": \"IE\", \"Name\": \"Ireland\"},{\"Code\": \"IM\", \"Name\": \"Isle of Man\"},{\"Code\": \"IL\", \"Name\": \"Israel\"},{\"Code\": \"IT\", \"Name\": \"Italy\"},{\"Code\": \"JM\", \"Name\": \"Jamaica\"},{\"Code\": \"JP\", \"Name\": \"Japan\"},{\"Code\": \"JE\", \"Name\": \"Jersey\"},{\"Code\": \"JO\", \"Name\": \"Jordan\"},{\"Code\": \"KZ\", \"Name\": \"Kazakhstan\"},{\"Code\": \"KE\", \"Name\": \"Kenya\"},{\"Code\": \"KI\", \"Name\": \"Kiribati\"},{\"Code\": \"KP\", \"Name\": \"Korea, Democratic People's Republic of\"},{\"Code\": \"KR\", \"Name\": \"Korea, Republic of\"},{\"Code\": \"KW\", \"Name\": \"Kuwait\"},{\"Code\": \"KG\", \"Name\": \"Kyrgyzstan\"},{\"Code\": \"LA\", \"Name\": \"Lao People's Democratic Republic\"},{\"Code\": \"LV\", \"Name\": \"Latvia\"},{\"Code\": \"LB\", \"Name\": \"Lebanon\"},{\"Code\": \"LS\", \"Name\": \"Lesotho\"},{\"Code\": \"LR\", \"Name\": \"Liberia\"},{\"Code\": \"LY\", \"Name\": \"Libya\"},{\"Code\": \"LI\", \"Name\": \"Liechtenstein\"},{\"Code\": \"LT\", \"Name\": \"Lithuania\"},{\"Code\": \"LU\", \"Name\": \"Luxembourg\"},{\"Code\": \"MO\", \"Name\": \"Macao\"},{\"Code\": \"MK\", \"Name\": \"Republic of North Macedonia\"},{\"Code\": \"MG\", \"Name\": \"Madagascar\"},{\"Code\": \"MW\", \"Name\": \"Malawi\"},{\"Code\": \"MY\", \"Name\": \"Malaysia\"},{\"Code\": \"MV\", \"Name\": \"Maldives\"},{\"Code\": \"ML\", \"Name\": \"Mali\"},{\"Code\": \"MT\", \"Name\": \"Malta\"},{\"Code\": \"MH\", \"Name\": \"Marshall Islands\"},{\"Code\": \"MQ\", \"Name\": \"Martinique\"},{\"Code\": \"MR\", \"Name\": \"Mauritania\"},{\"Code\": \"MU\", \"Name\": \"Mauritius\"},{\"Code\": \"YT\", \"Name\": \"Mayotte\"},{\"Code\": \"MX\", \"Name\": \"Mexico\"},{\"Code\": \"FM\", \"Name\": \"Micronesia, Federated States of\"},{\"Code\": \"MD\", \"Name\": \"Moldova, Republic of\"},{\"Code\": \"MC\", \"Name\": \"Monaco\"},{\"Code\": \"MN\", \"Name\": \"Mongolia\"},{\"Code\": \"ME\", \"Name\": \"Montenegro\"},{\"Code\": \"MS\", \"Name\": \"Montserrat\"},{\"Code\": \"MA\", \"Name\": \"Morocco\"},{\"Code\": \"MZ\", \"Name\": \"Mozambique\"},{\"Code\": \"MM\", \"Name\": \"Myanmar\"},{\"Code\": \"NA\", \"Name\": \"Namibia\"},{\"Code\": \"NR\", \"Name\": \"Nauru\"},{\"Code\": \"NP\", \"Name\": \"Nepal\"},{\"Code\": \"NL\", \"Name\": \"Netherlands\"},{\"Code\": \"NC\", \"Name\": \"New Caledonia\"},{\"Code\": \"NZ\", \"Name\": \"New Zealand\"},{\"Code\": \"NI\", \"Name\": \"Nicaragua\"},{\"Code\": \"NE\", \"Name\": \"Niger\"},{\"Code\": \"NG\", \"Name\": \"Nigeria\"},{\"Code\": \"NU\", \"Name\": \"Niue\"},{\"Code\": \"NF\", \"Name\": \"Norfolk Island\"},{\"Code\": \"MP\", \"Name\": \"Northern Mariana Islands\"},{\"Code\": \"NO\", \"Name\": \"Norway\"},{\"Code\": \"OM\", \"Name\": \"Oman\"},{\"Code\": \"PK\", \"Name\": \"Pakistan\"},{\"Code\": \"PW\", \"Name\": \"Palau\"},{\"Code\": \"PS\", \"Name\": \"Palestine, State of\"},{\"Code\": \"PA\", \"Name\": \"Panama\"},{\"Code\": \"PG\", \"Name\": \"Papua New Guinea\"},{\"Code\": \"PY\", \"Name\": \"Paraguay\"},{\"Code\": \"PE\", \"Name\": \"Peru\"},{\"Code\": \"PH\", \"Name\": \"Philippines\"},{\"Code\": \"PN\", \"Name\": \"Pitcairn\"},{\"Code\": \"PL\", \"Name\": \"Poland\"},{\"Code\": \"PT\", \"Name\": \"Portugal\"},{\"Code\": \"PR\", \"Name\": \"Puerto Rico\"},{\"Code\": \"QA\", \"Name\": \"Qatar\"},{\"Code\": \"RE\", \"Name\": \"R\u00e9union\"},{\"Code\": \"RO\", \"Name\": \"Romania\"},{\"Code\": \"RU\", \"Name\": \"Russian Federation\"},{\"Code\": \"RW\", \"Name\": \"Rwanda\"},{\"Code\": \"BL\", \"Name\": \"Saint Barth\u00e9lemy\"},{\"Code\": \"SH\", \"Name\": \"Saint Helena, Ascension and Tristan da Cunha\"},{\"Code\": \"KN\", \"Name\": \"Saint Kitts and Nevis\"},{\"Code\": \"LC\", \"Name\": \"Saint Lucia\"},{\"Code\": \"MF\", \"Name\": \"Saint Martin (French part)\"},{\"Code\": \"PM\", \"Name\": \"Saint Pierre and Miquelon\"},{\"Code\": \"VC\", \"Name\": \"Saint Vincent and the Grenadines\"},{\"Code\": \"WS\", \"Name\": \"Samoa\"},{\"Code\": \"SM\", \"Name\": \"San Marino\"},{\"Code\": \"ST\", \"Name\": \"Sao Tome and Principe\"},{\"Code\": \"SA\", \"Name\": \"Saudi Arabia\"},{\"Code\": \"SN\", \"Name\": \"Senegal\"},{\"Code\": \"RS\", \"Name\": \"Serbia\"},{\"Code\": \"SC\", \"Name\": \"Seychelles\"},{\"Code\": \"SL\", \"Name\": \"Sierra Leone\"},{\"Code\": \"SG\", \"Name\": \"Singapore\"},{\"Code\": \"SX\", \"Name\": \"Sint Maarten (Dutch part)\"},{\"Code\": \"SK\", \"Name\": \"Slovakia\"},{\"Code\": \"SI\", \"Name\": \"Slovenia\"},{\"Code\": \"SB\", \"Name\": \"Solomon Islands\"},{\"Code\": \"SO\", \"Name\": \"Somalia\"},{\"Code\": \"ZA\", \"Name\": \"South Africa\"},{\"Code\": \"GS\", \"Name\": \"South Georgia and the South Sandwich Islands\"},{\"Code\": \"SS\", \"Name\": \"South Sudan\"},{\"Code\": \"ES\", \"Name\": \"Spain\"},{\"Code\": \"LK\", \"Name\": \"Sri Lanka\"},{\"Code\": \"SD\", \"Name\": \"Sudan\"},{\"Code\": \"SR\", \"Name\": \"Suriname\"},{\"Code\": \"SJ\", \"Name\": \"Svalbard and Jan Mayen\"},{\"Code\": \"SZ\", \"Name\": \"Swaziland\"},{\"Code\": \"SE\", \"Name\": \"Sweden\"},{\"Code\": \"CH\", \"Name\": \"Switzerland\"},{\"Code\": \"SY\", \"Name\": \"Syrian Arab Republic\"},{\"Code\": \"TW\", \"Name\": \"Taiwan, Province of China\"},{\"Code\": \"TJ\", \"Name\": \"Tajikistan\"},{\"Code\": \"TZ\", \"Name\": \"Tanzania, United Republic of\"},{\"Code\": \"TH\", \"Name\": \"Thailand\"},{\"Code\": \"TL\", \"Name\": \"Timor-Leste\"},{\"Code\": \"TG\", \"Name\": \"Togo\"},{\"Code\": \"TK\", \"Name\": \"Tokelau\"},{\"Code\": \"TO\", \"Name\": \"Tonga\"},{\"Code\": \"TT\", \"Name\": \"Trinidad and Tobago\"},{\"Code\": \"TN\", \"Name\": \"Tunisia\"},{\"Code\": \"TR\", \"Name\": \"Turkey\"},{\"Code\": \"TM\", \"Name\": \"Turkmenistan\"},{\"Code\": \"TC\", \"Name\": \"Turks and Caicos Islands\"},{\"Code\": \"TV\", \"Name\": \"Tuvalu\"},{\"Code\": \"UG\", \"Name\": \"Uganda\"},{\"Code\": \"UA\", \"Name\": \"Ukraine\"},{\"Code\": \"AE\", \"Name\": \"United Arab Emirates\"},{\"Code\": \"GB\", \"Name\": \"United Kingdom\"},{\"Code\": \"US\", \"Name\": \"United States\"},{\"Code\": \"UM\", \"Name\": \"United States Minor Outlying Islands\"},{\"Code\": \"UY\", \"Name\": \"Uruguay\"},{\"Code\": \"UZ\", \"Name\": \"Uzbekistan\"},{\"Code\": \"VU\", \"Name\": \"Vanuatu\"},{\"Code\": \"VE\", \"Name\": \"Venezuela, Bolivarian Republic of\"},{\"Code\": \"VN\", \"Name\": \"Viet Nam\"},{\"Code\": \"VG\", \"Name\": \"Virgin Islands, British\"},{\"Code\": \"VI\", \"Name\": \"Virgin Islands, U.S.\"},{\"Code\": \"WF\", \"Name\": \"Wallis and Futuna\"},{\"Code\": \"EH\", \"Name\": \"Western Sahara\"},{\"Code\": \"YE\", \"Name\": \"Yemen\"},{\"Code\": \"ZM\", \"Name\": \"Zambia\"},{\"Code\": \"ZW\", \"Name\": \"Zimbabwe\"}]";

            return JsonConvert.DeserializeObject<List<Code2Name>>(str);
        }
        private class Code2Name
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        private List<CurrCode2Name> CurrencyList()
        {
            var str =
"[{\"AlphabeticCode\": \"AFN\", \"Currency\": \"Afghani\", \"Entity\": \"AFGHANISTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 971.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"\u00c5LAND ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ALL\", \"Currency\": \"Lek\", \"Entity\": \"ALBANIA\", \"MinorUnit\": \"2\", \"NumericCode\": 8.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"DZD\", \"Currency\": \"Algerian Dinar\", \"Entity\": \"ALGERIA\", \"MinorUnit\": \"2\", \"NumericCode\": 12.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"AMERICAN SAMOA\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"ANDORRA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AOA\", \"Currency\": \"Kwanza\", \"Entity\": \"ANGOLA\", \"MinorUnit\": \"2\", \"NumericCode\": 973.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"ANGUILLA\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": null, \"Currency\": \"No universal currency\", \"Entity\": \"ANTARCTICA\", \"MinorUnit\": null, \"NumericCode\": null, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"ANTIGUA AND BARBUDA\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ARS\", \"Currency\": \"Argentine Peso\", \"Entity\": \"ARGENTINA\", \"MinorUnit\": \"2\", \"NumericCode\": 32.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AMD\", \"Currency\": \"Armenian Dram\", \"Entity\": \"ARMENIA\", \"MinorUnit\": \"2\", \"NumericCode\": 51.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AWG\", \"Currency\": \"Aruban Florin\", \"Entity\": \"ARUBA\", \"MinorUnit\": \"2\", \"NumericCode\": 533.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"AUSTRALIA\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"AUSTRIA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AZN\", \"Currency\": \"Azerbaijan Manat\", \"Entity\": \"AZERBAIJAN\", \"MinorUnit\": \"2\", \"NumericCode\": 944.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BSD\", \"Currency\": \"Bahamian Dollar\", \"Entity\": \"BAHAMAS\", \"MinorUnit\": \"2\", \"NumericCode\": 44.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BHD\", \"Currency\": \"Bahraini Dinar\", \"Entity\": \"BAHRAIN\", \"MinorUnit\": \"3\", \"NumericCode\": 48.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BDT\", \"Currency\": \"Taka\", \"Entity\": \"BANGLADESH\", \"MinorUnit\": \"2\", \"NumericCode\": 50.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BBD\", \"Currency\": \"Barbados Dollar\", \"Entity\": \"BARBADOS\", \"MinorUnit\": \"2\", \"NumericCode\": 52.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BYN\", \"Currency\": \"Belarusian Ruble\", \"Entity\": \"BELARUS\", \"MinorUnit\": \"2\", \"NumericCode\": 933.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"BELGIUM\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BZD\", \"Currency\": \"Belize Dollar\", \"Entity\": \"BELIZE\", \"MinorUnit\": \"2\", \"NumericCode\": 84.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"BENIN\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BMD\", \"Currency\": \"Bermudian Dollar\", \"Entity\": \"BERMUDA\", \"MinorUnit\": \"2\", \"NumericCode\": 60.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"INR\", \"Currency\": \"Indian Rupee\", \"Entity\": \"BHUTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 356.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BTN\", \"Currency\": \"Ngultrum\", \"Entity\": \"BHUTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 64.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BOB\", \"Currency\": \"Boliviano\", \"Entity\": \"Bolivia, Plurinational State of\", \"MinorUnit\": \"2\", \"NumericCode\": 68.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BOV\", \"Currency\": \"Mvdol\", \"Entity\": \"BOLIVIA (PLURINATIONAL STATE OF)\", \"MinorUnit\": \"2\", \"NumericCode\": 984.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"BONAIRE, SINT EUSTATIUS AND SABA\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BAM\", \"Currency\": \"Convertible Mark\", \"Entity\": \"BOSNIA AND HERZEGOVINA\", \"MinorUnit\": \"2\", \"NumericCode\": 977.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BWP\", \"Currency\": \"Pula\", \"Entity\": \"BOTSWANA\", \"MinorUnit\": \"2\", \"NumericCode\": 72.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NOK\", \"Currency\": \"Norwegian Krone\", \"Entity\": \"BOUVET ISLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 578.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BRL\", \"Currency\": \"Brazilian Real\", \"Entity\": \"BRAZIL\", \"MinorUnit\": \"2\", \"NumericCode\": 986.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"BRITISH INDIAN OCEAN TERRITORY\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BND\", \"Currency\": \"Brunei Dollar\", \"Entity\": \"BRUNEI DARUSSALAM\", \"MinorUnit\": \"2\", \"NumericCode\": 96.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BGN\", \"Currency\": \"Bulgarian Lev\", \"Entity\": \"BULGARIA\", \"MinorUnit\": \"2\", \"NumericCode\": 975.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"BURKINA FASO\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"BIF\", \"Currency\": \"Burundi Franc\", \"Entity\": \"BURUNDI\", \"MinorUnit\": \"0\", \"NumericCode\": 108.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CVE\", \"Currency\": \"Cabo Verde Escudo\", \"Entity\": \"CApe VERDE\", \"MinorUnit\": \"2\", \"NumericCode\": 132.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KHR\", \"Currency\": \"Riel\", \"Entity\": \"CAMBODIA\", \"MinorUnit\": \"2\", \"NumericCode\": 116.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XAF\", \"Currency\": \"CFA Franc BEAC\", \"Entity\": \"CAMEROON\", \"MinorUnit\": \"0\", \"NumericCode\": 950.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CAD\", \"Currency\": \"Canadian Dollar\", \"Entity\": \"CANADA\", \"MinorUnit\": \"2\", \"NumericCode\": 124.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KYD\", \"Currency\": \"Cayman Islands Dollar\", \"Entity\": \"CAYMAN ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 136.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XAF\", \"Currency\": \"CFA Franc BEAC\", \"Entity\": \"CENTRAL AFRICAN REPUBLIC\", \"MinorUnit\": \"0\", \"NumericCode\": 950.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XAF\", \"Currency\": \"CFA Franc BEAC\", \"Entity\": \"CHAD\", \"MinorUnit\": \"0\", \"NumericCode\": 950.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CLP\", \"Currency\": \"Chilean Peso\", \"Entity\": \"CHILE\", \"MinorUnit\": \"0\", \"NumericCode\": 152.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CLF\", \"Currency\": \"Unidad de Fomento\", \"Entity\": \"CHILE\", \"MinorUnit\": \"4\", \"NumericCode\": 990.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CNY\", \"Currency\": \"Yuan Renminbi\", \"Entity\": \"CHINA\", \"MinorUnit\": \"2\", \"NumericCode\": 156.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"CHRISTMAS ISLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"COCOS (KEELING) ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"COP\", \"Currency\": \"Colombian Peso\", \"Entity\": \"COLOMBIA\", \"MinorUnit\": \"2\", \"NumericCode\": 170.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"COU\", \"Currency\": \"Unidad de Valor Real\", \"Entity\": \"COLOMBIA\", \"MinorUnit\": \"2\", \"NumericCode\": 970.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KMF\", \"Currency\": \"Comorian Franc\", \"Entity\": \"COMOROS\", \"MinorUnit\": \"0\", \"NumericCode\": 174.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CDF\", \"Currency\": \"Congolese Franc\", \"Entity\": \"CONGO, THE DEMOCRATIC REPUBLIC OF THE\", \"MinorUnit\": \"2\", \"NumericCode\": 976.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XAF\", \"Currency\": \"CFA Franc BEAC\", \"Entity\": \"CONGO\", \"MinorUnit\": \"0\", \"NumericCode\": 950.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NZD\", \"Currency\": \"New Zealand Dollar\", \"Entity\": \"COOK ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 554.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CRC\", \"Currency\": \"Costa Rican Colon\", \"Entity\": \"COSTA RICA\", \"MinorUnit\": \"2\", \"NumericCode\": 188.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"C\u00d4TE D'IVOIRE\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"HRK\", \"Currency\": \"Kuna\", \"Entity\": \"CROATIA\", \"MinorUnit\": \"2\", \"NumericCode\": 191.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CUP\", \"Currency\": \"Cuban Peso\", \"Entity\": \"CUBA\", \"MinorUnit\": \"2\", \"NumericCode\": 192.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CUC\", \"Currency\": \"Peso Convertible\", \"Entity\": \"CUBA\", \"MinorUnit\": \"2\", \"NumericCode\": 931.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ANG\", \"Currency\": \"Netherlands Antillean Guilder\", \"Entity\": \"CURA\u00c7AO\", \"MinorUnit\": \"2\", \"NumericCode\": 532.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"CYPRUS\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CZK\", \"Currency\": \"Czech Koruna\", \"Entity\": \"Czech Republic\", \"MinorUnit\": \"2\", \"NumericCode\": 203.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"DKK\", \"Currency\": \"Danish Krone\", \"Entity\": \"DENMARK\", \"MinorUnit\": \"2\", \"NumericCode\": 208.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"DJF\", \"Currency\": \"Djibouti Franc\", \"Entity\": \"DJIBOUTI\", \"MinorUnit\": \"0\", \"NumericCode\": 262.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"DOMINICA\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"DOP\", \"Currency\": \"Dominican Peso\", \"Entity\": \"DOMINICAN REPUBLIC\", \"MinorUnit\": \"2\", \"NumericCode\": 214.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"ECUADOR\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EGP\", \"Currency\": \"Egyptian Pound\", \"Entity\": \"EGYPT\", \"MinorUnit\": \"2\", \"NumericCode\": 818.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SVC\", \"Currency\": \"El Salvador Colon\", \"Entity\": \"EL SALVADOR\", \"MinorUnit\": \"2\", \"NumericCode\": 222.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"EL SALVADOR\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XAF\", \"Currency\": \"CFA Franc BEAC\", \"Entity\": \"EQUATORIAL GUINEA\", \"MinorUnit\": \"0\", \"NumericCode\": 950.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ERN\", \"Currency\": \"Nakfa\", \"Entity\": \"ERITREA\", \"MinorUnit\": \"2\", \"NumericCode\": 232.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"ESTONIA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SZL\", \"Currency\": \"Lilangeni\", \"Entity\": \"Swaziland\", \"MinorUnit\": \"2\", \"NumericCode\": 748.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ETB\", \"Currency\": \"Ethiopian Birr\", \"Entity\": \"ETHIOPIA\", \"MinorUnit\": \"2\", \"NumericCode\": 230.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"EUROPEAN UNION\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"FKP\", \"Currency\": \"Falkland Islands Pound\", \"Entity\": \"FALKLAND ISLANDS (MALVINAS)\", \"MinorUnit\": \"2\", \"NumericCode\": 238.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"DKK\", \"Currency\": \"Danish Krone\", \"Entity\": \"FAROE ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 208.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"FJD\", \"Currency\": \"Fiji Dollar\", \"Entity\": \"FIJI\", \"MinorUnit\": \"2\", \"NumericCode\": 242.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"FINLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"FRANCE\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"FRENCH GUIANA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XPF\", \"Currency\": \"CFP Franc\", \"Entity\": \"FRENCH POLYNESIA\", \"MinorUnit\": \"0\", \"NumericCode\": 953.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"FRENCH SOUTHERN TERRITORIES\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XAF\", \"Currency\": \"CFA Franc BEAC\", \"Entity\": \"GABON\", \"MinorUnit\": \"0\", \"NumericCode\": 950.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GMD\", \"Currency\": \"Dalasi\", \"Entity\": \"GAMBIA\", \"MinorUnit\": \"2\", \"NumericCode\": 270.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GEL\", \"Currency\": \"Lari\", \"Entity\": \"GEORGIA\", \"MinorUnit\": \"2\", \"NumericCode\": 981.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"GERMANY\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GHS\", \"Currency\": \"Ghana Cedi\", \"Entity\": \"GHANA\", \"MinorUnit\": \"2\", \"NumericCode\": 936.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GIP\", \"Currency\": \"Gibraltar Pound\", \"Entity\": \"GIBRALTAR\", \"MinorUnit\": \"2\", \"NumericCode\": 292.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"GREECE\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"DKK\", \"Currency\": \"Danish Krone\", \"Entity\": \"GREENLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 208.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"GRENADA\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"GUADELOUPE\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"GUAM\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GTQ\", \"Currency\": \"Quetzal\", \"Entity\": \"GUATEMALA\", \"MinorUnit\": \"2\", \"NumericCode\": 320.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GBP\", \"Currency\": \"Pound Sterling\", \"Entity\": \"GUERNSEY\", \"MinorUnit\": \"2\", \"NumericCode\": 826.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GNF\", \"Currency\": \"Guinean Franc\", \"Entity\": \"GUINEA\", \"MinorUnit\": \"0\", \"NumericCode\": 324.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"GUINEA-BISSAU\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GYD\", \"Currency\": \"Guyana Dollar\", \"Entity\": \"GUYANA\", \"MinorUnit\": \"2\", \"NumericCode\": 328.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"HTG\", \"Currency\": \"Gourde\", \"Entity\": \"HAITI\", \"MinorUnit\": \"2\", \"NumericCode\": 332.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"HAITI\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"HEARD ISLAND AND McDONALD ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"HOLY SEE (Vatican City State)\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"HNL\", \"Currency\": \"Lempira\", \"Entity\": \"HONDURAS\", \"MinorUnit\": \"2\", \"NumericCode\": 340.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"HKD\", \"Currency\": \"Hong Kong Dollar\", \"Entity\": \"HONG KONG\", \"MinorUnit\": \"2\", \"NumericCode\": 344.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"HUF\", \"Currency\": \"Forint\", \"Entity\": \"HUNGARY\", \"MinorUnit\": \"2\", \"NumericCode\": 348.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ISK\", \"Currency\": \"Iceland Krona\", \"Entity\": \"ICELAND\", \"MinorUnit\": \"0\", \"NumericCode\": 352.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"INR\", \"Currency\": \"Indian Rupee\", \"Entity\": \"INDIA\", \"MinorUnit\": \"2\", \"NumericCode\": 356.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"IDR\", \"Currency\": \"Rupiah\", \"Entity\": \"INDONESIA\", \"MinorUnit\": \"2\", \"NumericCode\": 360.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XDR\", \"Currency\": \"SDR (Special Drawing Right)\", \"Entity\": \"INTERNATIONAL MONETARY FUND (IMF)\", \"MinorUnit\": \"-\", \"NumericCode\": 960.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"IRR\", \"Currency\": \"Iranian Rial\", \"Entity\": \"IRAN, ISLAMIC REPUBLIC OF\", \"MinorUnit\": \"2\", \"NumericCode\": 364.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"IQD\", \"Currency\": \"Iraqi Dinar\", \"Entity\": \"IRAQ\", \"MinorUnit\": \"3\", \"NumericCode\": 368.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"IRELAND\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GBP\", \"Currency\": \"Pound Sterling\", \"Entity\": \"ISLE OF MAN\", \"MinorUnit\": \"2\", \"NumericCode\": 826.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ILS\", \"Currency\": \"New Israeli Sheqel\", \"Entity\": \"ISRAEL\", \"MinorUnit\": \"2\", \"NumericCode\": 376.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"ITALY\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"JMD\", \"Currency\": \"Jamaican Dollar\", \"Entity\": \"JAMAICA\", \"MinorUnit\": \"2\", \"NumericCode\": 388.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"JPY\", \"Currency\": \"Yen\", \"Entity\": \"JAPAN\", \"MinorUnit\": \"0\", \"NumericCode\": 392.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GBP\", \"Currency\": \"Pound Sterling\", \"Entity\": \"JERSEY\", \"MinorUnit\": \"2\", \"NumericCode\": 826.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"JOD\", \"Currency\": \"Jordanian Dinar\", \"Entity\": \"JORDAN\", \"MinorUnit\": \"3\", \"NumericCode\": 400.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KZT\", \"Currency\": \"Tenge\", \"Entity\": \"KAZAKHSTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 398.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KES\", \"Currency\": \"Kenyan Shilling\", \"Entity\": \"KENYA\", \"MinorUnit\": \"2\", \"NumericCode\": 404.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"KIRIBATI\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KPW\", \"Currency\": \"North Korean Won\", \"Entity\": \"KOREA, DEMOCRATIC PEOPLE'S REPUBLIC OF\", \"MinorUnit\": \"2\", \"NumericCode\": 408.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KRW\", \"Currency\": \"Won\", \"Entity\": \"KOREA, REPUBLIC OF\", \"MinorUnit\": \"0\", \"NumericCode\": 410.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KWD\", \"Currency\": \"Kuwaiti Dinar\", \"Entity\": \"KUWAIT\", \"MinorUnit\": \"3\", \"NumericCode\": 414.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"KGS\", \"Currency\": \"Som\", \"Entity\": \"KYRGYZSTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 417.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"LAK\", \"Currency\": \"Lao Kip\", \"Entity\": \"LAO PEOPLE'S DEMOCRATIC REPUBLIC\", \"MinorUnit\": \"2\", \"NumericCode\": 418.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"LATVIA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"LBP\", \"Currency\": \"Lebanese Pound\", \"Entity\": \"LEBANON\", \"MinorUnit\": \"2\", \"NumericCode\": 422.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"LSL\", \"Currency\": \"Loti\", \"Entity\": \"LESOTHO\", \"MinorUnit\": \"2\", \"NumericCode\": 426.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ZAR\", \"Currency\": \"Rand\", \"Entity\": \"LESOTHO\", \"MinorUnit\": \"2\", \"NumericCode\": 710.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"LRD\", \"Currency\": \"Liberian Dollar\", \"Entity\": \"LIBERIA\", \"MinorUnit\": \"2\", \"NumericCode\": 430.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"LYD\", \"Currency\": \"Libyan Dinar\", \"Entity\": \"LIBYA\", \"MinorUnit\": \"3\", \"NumericCode\": 434.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CHF\", \"Currency\": \"Swiss Franc\", \"Entity\": \"LIECHTENSTEIN\", \"MinorUnit\": \"2\", \"NumericCode\": 756.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"LITHUANIA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"LUXEMBOURG\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MOP\", \"Currency\": \"Pataca\", \"Entity\": \"MACAO\", \"MinorUnit\": \"2\", \"NumericCode\": 446.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MKD\", \"Currency\": \"Denar\", \"Entity\": \"Republic of North Macedonia\", \"MinorUnit\": \"2\", \"NumericCode\": 807.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MGA\", \"Currency\": \"Malagasy Ariary\", \"Entity\": \"MADAGASCAR\", \"MinorUnit\": \"2\", \"NumericCode\": 969.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MWK\", \"Currency\": \"Malawi Kwacha\", \"Entity\": \"MALAWI\", \"MinorUnit\": \"2\", \"NumericCode\": 454.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MYR\", \"Currency\": \"Malaysian Ringgit\", \"Entity\": \"MALAYSIA\", \"MinorUnit\": \"2\", \"NumericCode\": 458.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MVR\", \"Currency\": \"Rufiyaa\", \"Entity\": \"MALDIVES\", \"MinorUnit\": \"2\", \"NumericCode\": 462.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"MALI\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"MALTA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"MARSHALL ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"MARTINIQUE\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MRU\", \"Currency\": \"Ouguiya\", \"Entity\": \"MAURITANIA\", \"MinorUnit\": \"2\", \"NumericCode\": 929.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MUR\", \"Currency\": \"Mauritius Rupee\", \"Entity\": \"MAURITIUS\", \"MinorUnit\": \"2\", \"NumericCode\": 480.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"MAYOTTE\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XUA\", \"Currency\": \"ADB Unit of Account\", \"Entity\": \"MEMBER COUNTRIES OF THE AFRICAN DEVELOPMENT BANK GROUP\", \"MinorUnit\": \"-\", \"NumericCode\": 965.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MXN\", \"Currency\": \"Mexican Peso\", \"Entity\": \"MEXICO\", \"MinorUnit\": \"2\", \"NumericCode\": 484.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MXV\", \"Currency\": \"Mexican Unidad de Inversion (UDI)\", \"Entity\": \"MEXICO\", \"MinorUnit\": \"2\", \"NumericCode\": 979.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"MICRONESIA, FEDERATED STATES OF\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MDL\", \"Currency\": \"Moldovan Leu\", \"Entity\": \"MOLDOVA, REPUBLIC OF\", \"MinorUnit\": \"2\", \"NumericCode\": 498.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"MONACO\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MNT\", \"Currency\": \"Tugrik\", \"Entity\": \"MONGOLIA\", \"MinorUnit\": \"2\", \"NumericCode\": 496.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"MONTENEGRO\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"MONTSERRAT\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MAD\", \"Currency\": \"Moroccan Dirham\", \"Entity\": \"MOROCCO\", \"MinorUnit\": \"2\", \"NumericCode\": 504.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MZN\", \"Currency\": \"Mozambique Metical\", \"Entity\": \"MOZAMBIQUE\", \"MinorUnit\": \"2\", \"NumericCode\": 943.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MMK\", \"Currency\": \"Kyat\", \"Entity\": \"MYANMAR\", \"MinorUnit\": \"2\", \"NumericCode\": 104.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NAD\", \"Currency\": \"Namibia Dollar\", \"Entity\": \"NAMIBIA\", \"MinorUnit\": \"2\", \"NumericCode\": 516.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ZAR\", \"Currency\": \"Rand\", \"Entity\": \"NAMIBIA\", \"MinorUnit\": \"2\", \"NumericCode\": 710.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"NAURU\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NPR\", \"Currency\": \"Nepalese Rupee\", \"Entity\": \"NEPAL\", \"MinorUnit\": \"2\", \"NumericCode\": 524.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"NETHERLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XPF\", \"Currency\": \"CFP Franc\", \"Entity\": \"NEW CALEDONIA\", \"MinorUnit\": \"0\", \"NumericCode\": 953.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NZD\", \"Currency\": \"New Zealand Dollar\", \"Entity\": \"NEW ZEALAND\", \"MinorUnit\": \"2\", \"NumericCode\": 554.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NIO\", \"Currency\": \"Cordoba Oro\", \"Entity\": \"NICARAGUA\", \"MinorUnit\": \"2\", \"NumericCode\": 558.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"NIGER\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NGN\", \"Currency\": \"Naira\", \"Entity\": \"NIGERIA\", \"MinorUnit\": \"2\", \"NumericCode\": 566.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NZD\", \"Currency\": \"New Zealand Dollar\", \"Entity\": \"NIUE\", \"MinorUnit\": \"2\", \"NumericCode\": 554.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"NORFOLK ISLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"NORTHERN MARIANA ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NOK\", \"Currency\": \"Norwegian Krone\", \"Entity\": \"NORWAY\", \"MinorUnit\": \"2\", \"NumericCode\": 578.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"OMR\", \"Currency\": \"Rial Omani\", \"Entity\": \"OMAN\", \"MinorUnit\": \"3\", \"NumericCode\": 512.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"PKR\", \"Currency\": \"Pakistan Rupee\", \"Entity\": \"PAKISTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 586.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"PALAU\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": null, \"Currency\": \"No universal currency\", \"Entity\": \"PALESTINE, STATE OF\", \"MinorUnit\": null, \"NumericCode\": null, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"PAB\", \"Currency\": \"Balboa\", \"Entity\": \"PANAMA\", \"MinorUnit\": \"2\", \"NumericCode\": 590.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"PANAMA\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"PGK\", \"Currency\": \"Kina\", \"Entity\": \"PAPUA NEW GUINEA\", \"MinorUnit\": \"2\", \"NumericCode\": 598.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"PYG\", \"Currency\": \"Guarani\", \"Entity\": \"PARAGUAY\", \"MinorUnit\": \"0\", \"NumericCode\": 600.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"PEN\", \"Currency\": \"Sol\", \"Entity\": \"PERU\", \"MinorUnit\": \"2\", \"NumericCode\": 604.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"PHP\", \"Currency\": \"Philippine Peso\", \"Entity\": \"PHILIPPINES\", \"MinorUnit\": \"2\", \"NumericCode\": 608.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NZD\", \"Currency\": \"New Zealand Dollar\", \"Entity\": \"PITCAIRN\", \"MinorUnit\": \"2\", \"NumericCode\": 554.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"PLN\", \"Currency\": \"Zloty\", \"Entity\": \"POLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 985.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"PORTUGAL\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"PUERTO RICO\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"QAR\", \"Currency\": \"Qatari Rial\", \"Entity\": \"QATAR\", \"MinorUnit\": \"2\", \"NumericCode\": 634.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"R\u00c9UNION\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"RON\", \"Currency\": \"Romanian Leu\", \"Entity\": \"ROMANIA\", \"MinorUnit\": \"2\", \"NumericCode\": 946.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"RUB\", \"Currency\": \"Russian Ruble\", \"Entity\": \"RUSSIAN FEDERATION\", \"MinorUnit\": \"2\", \"NumericCode\": 643.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"RWF\", \"Currency\": \"Rwanda Franc\", \"Entity\": \"RWANDA\", \"MinorUnit\": \"0\", \"NumericCode\": 646.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"SAINT BARTH\u00c9LEMY\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SHP\", \"Currency\": \"Saint Helena Pound\", \"Entity\": \"SAINT HELENA, ASCENSION AND TRISTAN DA CUNHA\", \"MinorUnit\": \"2\", \"NumericCode\": 654.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"SAINT KITTS AND NEVIS\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"SAINT LUCIA\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"SAINT MARTIN (FRENCH PART)\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"SAINT PIERRE AND MIQUELON\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XCD\", \"Currency\": \"East Caribbean Dollar\", \"Entity\": \"SAINT VINCENT AND THE GRENADINES\", \"MinorUnit\": \"2\", \"NumericCode\": 951.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"WST\", \"Currency\": \"Tala\", \"Entity\": \"SAMOA\", \"MinorUnit\": \"2\", \"NumericCode\": 882.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"SAN MARINO\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"STN\", \"Currency\": \"Dobra\", \"Entity\": \"SAO TOME AND PRINCIPE\", \"MinorUnit\": \"2\", \"NumericCode\": 930.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SAR\", \"Currency\": \"Saudi Riyal\", \"Entity\": \"SAUDI ARABIA\", \"MinorUnit\": \"2\", \"NumericCode\": 682.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"SENEGAL\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"RSD\", \"Currency\": \"Serbian Dinar\", \"Entity\": \"SERBIA\", \"MinorUnit\": \"2\", \"NumericCode\": 941.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SCR\", \"Currency\": \"Seychelles Rupee\", \"Entity\": \"SEYCHELLES\", \"MinorUnit\": \"2\", \"NumericCode\": 690.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SLL\", \"Currency\": \"Leone\", \"Entity\": \"SIERRA LEONE\", \"MinorUnit\": \"2\", \"NumericCode\": 694.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SGD\", \"Currency\": \"Singapore Dollar\", \"Entity\": \"SINGAPORE\", \"MinorUnit\": \"2\", \"NumericCode\": 702.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ANG\", \"Currency\": \"Netherlands Antillean Guilder\", \"Entity\": \"SINT MAARTEN (DUTCH PART)\", \"MinorUnit\": \"2\", \"NumericCode\": 532.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"SLOVAKIA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"SLOVENIA\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SBD\", \"Currency\": \"Solomon Islands Dollar\", \"Entity\": \"SOLOMON ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 90.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SOS\", \"Currency\": \"Somali Shilling\", \"Entity\": \"SOMALIA\", \"MinorUnit\": \"2\", \"NumericCode\": 706.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ZAR\", \"Currency\": \"Rand\", \"Entity\": \"SOUTH AFRICA\", \"MinorUnit\": \"2\", \"NumericCode\": 710.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": null, \"Currency\": \"No universal currency\", \"Entity\": \"SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS\", \"MinorUnit\": null, \"NumericCode\": null, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SSP\", \"Currency\": \"South Sudanese Pound\", \"Entity\": \"SOUTH SUDAN\", \"MinorUnit\": \"2\", \"NumericCode\": 728.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"EUR\", \"Currency\": \"Euro\", \"Entity\": \"SPAIN\", \"MinorUnit\": \"2\", \"NumericCode\": 978.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"LKR\", \"Currency\": \"Sri Lanka Rupee\", \"Entity\": \"SRI LANKA\", \"MinorUnit\": \"2\", \"NumericCode\": 144.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SDG\", \"Currency\": \"Sudanese Pound\", \"Entity\": \"SUDAN\", \"MinorUnit\": \"2\", \"NumericCode\": 938.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SRD\", \"Currency\": \"Surinam Dollar\", \"Entity\": \"SURINAME\", \"MinorUnit\": \"2\", \"NumericCode\": 968.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NOK\", \"Currency\": \"Norwegian Krone\", \"Entity\": \"SVALBARD AND JAN MAYEN\", \"MinorUnit\": \"2\", \"NumericCode\": 578.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SEK\", \"Currency\": \"Swedish Krona\", \"Entity\": \"SWEDEN\", \"MinorUnit\": \"2\", \"NumericCode\": 752.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CHF\", \"Currency\": \"Swiss Franc\", \"Entity\": \"SWITZERLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 756.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CHE\", \"Currency\": \"WIR Euro\", \"Entity\": \"SWITZERLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 947.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"CHW\", \"Currency\": \"WIR Franc\", \"Entity\": \"SWITZERLAND\", \"MinorUnit\": \"2\", \"NumericCode\": 948.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"SYP\", \"Currency\": \"Syrian Pound\", \"Entity\": \"SYRIAN ARAB REPUBLIC\", \"MinorUnit\": \"2\", \"NumericCode\": 760.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TWD\", \"Currency\": \"New Taiwan Dollar\", \"Entity\": \"TAIWAN, PROVINCE OF CHINA\", \"MinorUnit\": \"2\", \"NumericCode\": 901.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TJS\", \"Currency\": \"Somoni\", \"Entity\": \"TAJIKISTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 972.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TZS\", \"Currency\": \"Tanzanian Shilling\", \"Entity\": \"TANZANIA, UNITED REPUBLIC OF\", \"MinorUnit\": \"2\", \"NumericCode\": 834.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"THB\", \"Currency\": \"Baht\", \"Entity\": \"THAILAND\", \"MinorUnit\": \"2\", \"NumericCode\": 764.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"TIMOR-LESTE\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XOF\", \"Currency\": \"CFA Franc BCEAO\", \"Entity\": \"TOGO\", \"MinorUnit\": \"0\", \"NumericCode\": 952.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"NZD\", \"Currency\": \"New Zealand Dollar\", \"Entity\": \"TOKELAU\", \"MinorUnit\": \"2\", \"NumericCode\": 554.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TOP\", \"Currency\": \"Pa'anga\", \"Entity\": \"TONGA\", \"MinorUnit\": \"2\", \"NumericCode\": 776.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TTD\", \"Currency\": \"Trinidad and Tobago Dollar\", \"Entity\": \"TRINIDAD AND TOBAGO\", \"MinorUnit\": \"2\", \"NumericCode\": 780.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TND\", \"Currency\": \"Tunisian Dinar\", \"Entity\": \"TUNISIA\", \"MinorUnit\": \"3\", \"NumericCode\": 788.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TRY\", \"Currency\": \"Turkish Lira\", \"Entity\": \"TURKEY\", \"MinorUnit\": \"2\", \"NumericCode\": 949.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"TMT\", \"Currency\": \"Turkmenistan New Manat\", \"Entity\": \"TURKMENISTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 934.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"TURKS AND CAICOS ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AUD\", \"Currency\": \"Australian Dollar\", \"Entity\": \"TUVALU\", \"MinorUnit\": \"2\", \"NumericCode\": 36.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"UGX\", \"Currency\": \"Uganda Shilling\", \"Entity\": \"UGANDA\", \"MinorUnit\": \"0\", \"NumericCode\": 800.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"UAH\", \"Currency\": \"Hryvnia\", \"Entity\": \"UKRAINE\", \"MinorUnit\": \"2\", \"NumericCode\": 980.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"AED\", \"Currency\": \"UAE Dirham\", \"Entity\": \"UNITED ARAB EMIRATES\", \"MinorUnit\": \"2\", \"NumericCode\": 784.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"GBP\", \"Currency\": \"Pound Sterling\", \"Entity\": \"UNITED KINGDOM\", \"MinorUnit\": \"2\", \"NumericCode\": 826.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"UNITED STATES MINOR OUTLYING ISLANDS\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"UNITED STATES\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USN\", \"Currency\": \"US Dollar (Next day)\", \"Entity\": \"UNITED STATES OF AMERICA\", \"MinorUnit\": \"2\", \"NumericCode\": 997.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"UYU\", \"Currency\": \"Peso Uruguayo\", \"Entity\": \"URUGUAY\", \"MinorUnit\": \"2\", \"NumericCode\": 858.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"UYI\", \"Currency\": \"Uruguay Peso en Unidades Indexadas (UI)\", \"Entity\": \"URUGUAY\", \"MinorUnit\": \"0\", \"NumericCode\": 940.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"UYW\", \"Currency\": \"Unidad Previsional\", \"Entity\": \"URUGUAY\", \"MinorUnit\": \"4\", \"NumericCode\": 927.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"UZS\", \"Currency\": \"Uzbekistan Sum\", \"Entity\": \"UZBEKISTAN\", \"MinorUnit\": \"2\", \"NumericCode\": 860.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"VUV\", \"Currency\": \"Vatu\", \"Entity\": \"VANUATU\", \"MinorUnit\": \"0\", \"NumericCode\": 548.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"VES\", \"Currency\": \"Bol\u00edvar Soberano\", \"Entity\": \"VENEZUELA, BOLIVARIAN REPUBLIC OF\", \"MinorUnit\": \"2\", \"NumericCode\": 928.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"VND\", \"Currency\": \"Dong\", \"Entity\": \"VIET NAM\", \"MinorUnit\": \"0\", \"NumericCode\": 704.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"VIRGIN ISLANDS, BRITISH\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"USD\", \"Currency\": \"US Dollar\", \"Entity\": \"VIRGIN ISLANDS, U.S.\", \"MinorUnit\": \"2\", \"NumericCode\": 840.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"XPF\", \"Currency\": \"CFP Franc\", \"Entity\": \"WALLIS AND FUTUNA\", \"MinorUnit\": \"0\", \"NumericCode\": 953.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"MAD\", \"Currency\": \"Moroccan Dirham\", \"Entity\": \"WESTERN SAHARA\", \"MinorUnit\": \"2\", \"NumericCode\": 504.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"YER\", \"Currency\": \"Yemeni Rial\", \"Entity\": \"YEMEN\", \"MinorUnit\": \"2\", \"NumericCode\": 886.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ZMW\", \"Currency\": \"Zambian Kwacha\", \"Entity\": \"ZAMBIA\", \"MinorUnit\": \"2\", \"NumericCode\": 967.0, \"WithdrawalDate\": null},{\"AlphabeticCode\": \"ZWL\", \"Currency\": \"Zimbabwe Dollar\", \"Entity\": \"ZIMBABWE\", \"MinorUnit\": \"2\", \"NumericCode\": 932.0, \"WithdrawalDate\": null}]"
;

                return JsonConvert.DeserializeObject<List<CurrCode2Name>>(str);
        }

        private class CurrCode2Name
        {
            public string AlphabeticCode { get; set; }
            public string Currency { get; set; }
            public string Entity { get; set; }
        }

        private IEnumerable<CountryCodeNameCurrency> GetCountryCodeNameCurrencies()
        {
            var country2Code = AllCountryCode2Name();
            var curr2Code = CurrencyList();

            foreach (var cc in country2Code)
            {
                var currCode = curr2Code
                    .FirstOrDefault(x => x.Entity.ToUpper() == cc.Name.ToUpper())
                    ?.AlphabeticCode
                    ?? "USD";

                yield return new CountryCodeNameCurrency
                {
                    CountryCode = cc.Code,
                    Name = cc.Name,
                    CurrCode = currCode
                };
            }
        }

        [DebuggerDisplay("{Name} - {CountryCode} - {CurrCode}")]
        private class CountryCodeNameCurrency
        {
            public string CountryCode { get; set; }
            public string Name { get; set; }
            public string CurrCode { get; set; }
        }
    }
}
