using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.SearchEngine.Engine.ApiRequesters;
using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models;
using FlyAnytime.Tools;
using Moq;
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace FlyAnytime.SearchEngine.Tests
{
    public class EngineTest
    {
        #region Location codes

        private static string Kyiv = "IEV";
        private static string NY = "NYC";
        private static string London = "LON";

        private static string CacheKey_IEV2NY = $"{Kyiv}-{NY}";
        private static string CacheKey_IEV2LON = $"{Kyiv}-{London}";

        private static string KyivAir1 = "KBP";
        private static string KyivAir2 = "IEV";
        private static string NyAir1 = "JFK";
        private static string NyAir2 = "LGA";
        private static string NyAir3 = "EWR";
        private static string LondAir1 = "LGW";
        private static string LondAir2 = "STN";

        private static Dictionary<string, string>  airport2CityMap = new Dictionary<string, string>
        {
            {KyivAir1, Kyiv},
            {KyivAir2, Kyiv},
            {NyAir1, NY},
            {NyAir2, NY},
            {NyAir3, NY},
            {LondAir1, London},
            {LondAir2, London},
        };

        #endregion

        #region Currency

        private static string EUR = "EUR";
        private static string USD = "USD";
        private static string UAH = "UAH";

        private static Dictionary<string, decimal> currencyExchange2EurMap = new Dictionary<string, decimal>
        {
            { EUR, 1 },
            { USD, 0.8M },
            { UAH, 31 },
        };

        #endregion

        #region DateTime

        private static DateTime flyStart = new DateTime(2021, 12, 4);
        private static DateTime flyEnd = new DateTime(2022, 5, 3);

        private static DateTime mon4AM = new DateTime(2021, 12, 20, 4, 34, 0, DateTimeKind.Utc);
        private static DateTime mon11AM = new DateTime(2021, 12, 20, 11, 34, 0, DateTimeKind.Utc);
        private static DateTime mon5PM = new DateTime(2021, 12, 20, 17, 34, 0, DateTimeKind.Utc);

        private static DateTime tue4AM = mon4AM.AddDays(1);
        private static DateTime tue11AM = mon11AM.AddDays(1);
        private static DateTime tue5PM = mon5PM.AddDays(1);

        private static DateTime wen4AM = mon4AM.AddDays(2);
        private static DateTime wen11AM = mon11AM.AddDays(2);
        private static DateTime wen5PM = mon5PM.AddDays(2);

        private static DateTime thur4AM = mon4AM.AddDays(3);
        private static DateTime thur11AM = mon11AM.AddDays(3);
        private static DateTime thur5PM = mon5PM.AddDays(3);

        private static DateTime frd4AM = mon4AM.AddDays(4);
        private static DateTime frd11AM = mon11AM.AddDays(4);
        private static DateTime frd5PM = mon5PM.AddDays(4);

        private static DateTime sat4AM = mon4AM.AddDays(5);
        private static DateTime sat11AM = mon11AM.AddDays(5);
        private static DateTime sat5PM = mon5PM.AddDays(5);

        private static DateTime sun4AM = mon4AM.AddDays(6);
        private static DateTime sun11AM = mon11AM.AddDays(6);
        private static DateTime sun5PM = mon5PM.AddDays(6);

        private static HashSet<byte> allDay = new HashSet<byte> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        private static HashSet<byte> earlyMorning = new HashSet<byte> { 0, 1, 2, 3, 4, 5, 6 };
        private static HashSet<byte> evening = new HashSet<byte> { 17, 18, 19, 20, 21, 22, 23 };
        private static HashSet<byte> midDay = new HashSet<byte> { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        #endregion

        [Fact]
        public async Task SearchEngine_OneAirpOneCity_FixPrice_AllSlots_Test()
        {
            var priceSettings = new PriceSettings(SearchPriceSettingsType.FixPrice, 500, USD);

            var flyDays = new Dictionary<Days, HashSet<byte>>
            {
                { Days.Monday, allDay },
                { Days.Tuesday, allDay },
                { Days.Wednesday, allDay },
                { Days.Thursday, allDay },
                { Days.Friday, allDay },
                { Days.Saturday, allDay },
                { Days.Sunday, allDay },
            };

            var backDays = new Dictionary<Days, HashSet<byte>>
            {
                { Days.Monday, allDay },
                { Days.Tuesday, allDay },
                { Days.Wednesday, allDay },
                { Days.Thursday, allDay },
                { Days.Friday, allDay },
                { Days.Saturday, allDay },
                { Days.Sunday, allDay },
            };

            var frame = new SearchFrame(flyStart.ToUtcUnix(), flyEnd.ToUtcUnix(), flyDays, backDays);

            var searchParams = new MakeSearchMessage(0, Kyiv, new[] { NyAir1, LondAir1 }, priceSettings, new TripDuration(3, 5), frame);

            var priceCache2value = new Dictionary<string, decimal>
            {
                { CacheKey_IEV2NY, 789M },
                { CacheKey_IEV2LON, 25M },
            };

            var result2NY = new List<ApiResultModel>
            {
                CreateRequstResult(400, USD, NY, frd11AM, tue4AM),
                CreateRequstResult(450, USD, NY, thur5PM, sun11AM),
                CreateRequstResult(460, USD, NY, sat5PM, wen5PM),
                CreateRequstResult(470, USD, NY, frd4AM, tue4AM),
                CreateRequstResult(475, USD, NY, mon11AM, frd4AM),
                CreateRequstResult(500, USD, NY, frd4AM, tue4AM),
                CreateRequstResult(600, USD, NY, frd4AM, tue4AM),
            };

            var result2Lon = new List<ApiResultModel>
            {
                CreateRequstResult(15, USD, London, frd11AM, tue4AM),
                CreateRequstResult(20, USD, London, thur5PM, sun11AM),
                CreateRequstResult(25, USD, London, sat5PM, wen5PM),
                CreateRequstResult(30, USD, London, mon11AM, frd4AM),
                CreateRequstResult(35, USD, London, frd4AM, tue4AM),
            };

            var res = new Dictionary<string, List<ApiResultModel>>
            {
                { CacheKey_IEV2LON, result2Lon },
                { CacheKey_IEV2NY, result2NY },
            };

            var apiMock = GetMockApiRequester(res);
            var cacheMock = GetMockForICacheHelper(priceCache2value);
            var channelMock = GetMockChannel();

            var engine = new Engine.SearchEngine(apiMock.Object, cacheMock.Object, channelMock.Object);
            var results = await engine.Search(searchParams);

            channelMock.Verify(x => x.WriteAsync(It.IsAny<ApiResultModel>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            apiMock.Verify(x => x.CreateRequest(It.IsAny<string>()), Times.Exactly(2));

            var lonResActual = results.Where(x => x.CityTo == London).ToList();
            var nyResActual = results.Where(x => x.CityTo == NY).ToList();

            Assert.True(3 == lonResActual.Count, "London res count is not 3");
            Assert.True(3 == nyResActual.Count, "NY res count is not 3");

            AssertSearchRes(lonResActual[0], result2Lon[0], priceCache2value[CacheKey_IEV2LON]);
            AssertSearchRes(lonResActual[1], result2Lon[1], priceCache2value[CacheKey_IEV2LON]);
            AssertSearchRes(lonResActual[2], result2Lon[2], priceCache2value[CacheKey_IEV2LON]);

            AssertSearchRes(nyResActual[0], result2NY[0], priceCache2value[CacheKey_IEV2NY]);
            AssertSearchRes(nyResActual[1], result2NY[1], priceCache2value[CacheKey_IEV2NY]);
            AssertSearchRes(nyResActual[2], result2NY[2], priceCache2value[CacheKey_IEV2NY]);
        }

        [Fact]
        public async Task SearchEngine_OneAirpOneCity_FixPrice_FilterBySlots_Test()
        {
            var priceSettings = new PriceSettings(SearchPriceSettingsType.FixPrice, 500, USD);

            var flyDays = new Dictionary<Days, HashSet<byte>>
            {
                { Days.Thursday, evening },
                { Days.Friday, earlyMorning },
            };

            var backDays = new Dictionary<Days, HashSet<byte>>
            {
                { Days.Sunday, midDay },
                { Days.Monday, evening },
                { Days.Tuesday, earlyMorning },
            };

            var frame = new SearchFrame(flyStart.ToUtcUnix(), flyEnd.ToUtcUnix(), flyDays, backDays);

            var searchParams = new MakeSearchMessage(0, Kyiv, new[] { NyAir1, LondAir1 }, priceSettings, new TripDuration(3, 5), frame);

            var priceCache2value = new Dictionary<string, decimal>
            {
                { CacheKey_IEV2NY, 789M },
                { CacheKey_IEV2LON, 25M },
            };

            var result2NY = new List<ApiResultModel>
            {
                CreateRequstResult(400, USD, NY, frd11AM, tue4AM), //-
                CreateRequstResult(450, USD, NY, thur5PM, sun11AM), //+
                CreateRequstResult(460, USD, NY, sat5PM, wen5PM), //-
                CreateRequstResult(470, USD, NY, frd4AM, tue4AM),//+
                CreateRequstResult(475, USD, NY, mon11AM, frd4AM),//-
                CreateRequstResult(500, USD, NY, frd4AM, tue4AM),//+
                CreateRequstResult(600, USD, NY, frd4AM, tue4AM),//-
            };

            var result2Lon = new List<ApiResultModel>
            {
                CreateRequstResult(15, USD, London, frd11AM, tue4AM),//-
                CreateRequstResult(20, USD, London, thur5PM, sun11AM),//+
                CreateRequstResult(25, USD, London, sat5PM, wen5PM),//-
                CreateRequstResult(30, USD, London, mon11AM, frd4AM),//-
                CreateRequstResult(35, USD, London, frd4AM, tue4AM),//+
            };

            var res = new Dictionary<string, List<ApiResultModel>>
            {
                { CacheKey_IEV2LON, result2Lon },
                { CacheKey_IEV2NY, result2NY },
            };

            var apiMock = GetMockApiRequester(res);
            var cacheMock = GetMockForICacheHelper(priceCache2value);
            var channelMock = GetMockChannel();

            var engine = new Engine.SearchEngine(apiMock.Object, cacheMock.Object, channelMock.Object);
            var results = await engine.Search(searchParams);

            channelMock.Verify(x => x.WriteAsync(It.IsAny<ApiResultModel>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            apiMock.Verify(x => x.CreateRequest(It.IsAny<string>()), Times.Exactly(2));

            var lonResActual = results.Where(x => x.CityTo == London).ToList();
            var nyResActual = results.Where(x => x.CityTo == NY).ToList();

            Assert.True(2 == lonResActual.Count, "London res count is not 2");
            Assert.True(3 == nyResActual.Count, "NY res count is not 3");

            AssertSearchRes(lonResActual[0], result2Lon[1], priceCache2value[CacheKey_IEV2LON]);
            AssertSearchRes(lonResActual[1], result2Lon[4], priceCache2value[CacheKey_IEV2LON]);

            AssertSearchRes(nyResActual[0], result2NY[1], priceCache2value[CacheKey_IEV2NY]);
            AssertSearchRes(nyResActual[1], result2NY[3], priceCache2value[CacheKey_IEV2NY]);
            AssertSearchRes(nyResActual[2], result2NY[5], priceCache2value[CacheKey_IEV2NY]);
        }

        [Fact]
        public async Task SearchEngine_OneAirpOneCity_FixPrice_AllSlots_FilterByMaxPrice_Test()
        {
            var priceSettings = new PriceSettings(SearchPriceSettingsType.FixPrice, 500, USD);

            var flyDays = new Dictionary<Days, HashSet<byte>>
            {
                { Days.Monday, allDay },
                { Days.Tuesday, allDay },
                { Days.Wednesday, allDay },
                { Days.Thursday, allDay },
                { Days.Friday, allDay },
                { Days.Saturday, allDay },
                { Days.Sunday, allDay },
            };

            var backDays = new Dictionary<Days, HashSet<byte>>
            {
                { Days.Monday, allDay },
                { Days.Tuesday, allDay },
                { Days.Wednesday, allDay },
                { Days.Thursday, allDay },
                { Days.Friday, allDay },
                { Days.Saturday, allDay },
                { Days.Sunday, allDay },
            };

            var frame = new SearchFrame(flyStart.ToUtcUnix(), flyEnd.ToUtcUnix(), flyDays, backDays);

            var searchParams = new MakeSearchMessage(0, Kyiv, new[] { NyAir1, LondAir1 }, priceSettings, new TripDuration(3, 5), frame);

            var priceCache2value = new Dictionary<string, decimal>
            {
                { CacheKey_IEV2NY, 789M },
                { CacheKey_IEV2LON, 25M },
            };

            var result2NY = new List<ApiResultModel>
            {
                CreateRequstResult(450, USD, NY, frd4AM, tue4AM),
                CreateRequstResult(475, USD, NY, mon11AM, frd4AM),
                CreateRequstResult(510, USD, NY, frd4AM, tue4AM),
                CreateRequstResult(600, USD, NY, frd4AM, tue4AM),
            };

            var result2Lon = new List<ApiResultModel>
            {
                CreateRequstResult(15, USD, London, frd11AM, tue4AM),
                CreateRequstResult(20, USD, London, thur5PM, sun11AM),
                CreateRequstResult(25, USD, London, sat5PM, wen5PM),
                CreateRequstResult(30, USD, London, mon11AM, frd4AM),
                CreateRequstResult(35, USD, London, frd4AM, tue4AM),
            };

            var res = new Dictionary<string, List<ApiResultModel>>
            {
                { CacheKey_IEV2LON, result2Lon },
                { CacheKey_IEV2NY, result2NY },
            };

            var apiMock = GetMockApiRequester(res);
            var cacheMock = GetMockForICacheHelper(priceCache2value);
            var channelMock = GetMockChannel();

            var engine = new Engine.SearchEngine(apiMock.Object, cacheMock.Object, channelMock.Object);
            var results = await engine.Search(searchParams);

            channelMock.Verify(x => x.WriteAsync(It.IsAny<ApiResultModel>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            apiMock.Verify(x => x.CreateRequest(It.IsAny<string>()), Times.Exactly(2));

            var lonResActual = results.Where(x => x.CityTo == London).ToList();
            var nyResActual = results.Where(x => x.CityTo == NY).ToList();

            Assert.True(3 == lonResActual.Count, "London res count is not 3");
            Assert.True(2 == nyResActual.Count, "NY res count is not 2");

            AssertSearchRes(lonResActual[0], result2Lon[0], priceCache2value[CacheKey_IEV2LON]);
            AssertSearchRes(lonResActual[1], result2Lon[1], priceCache2value[CacheKey_IEV2LON]);
            AssertSearchRes(lonResActual[2], result2Lon[2], priceCache2value[CacheKey_IEV2LON]);

            AssertSearchRes(nyResActual[0], result2NY[0], priceCache2value[CacheKey_IEV2NY]);
            AssertSearchRes(nyResActual[1], result2NY[1], priceCache2value[CacheKey_IEV2NY]);
        }

        private ApiResultModel CreateRequstResult(decimal price, string curr, string flyTo, DateTime departure, DateTime arrival)
        {
            return new ApiResultModel
            {
                Price = price,
                Currency = curr,
                PriceInEur = price/currencyExchange2EurMap[curr],
                DepartureFromDateTimeLocal = departure.ToUtcUnix(),
                ArrivalBackDateTimeLocal = arrival.ToUtcUnix(),
                CityCodeTo = flyTo
            };
        }

        #region Moq

        private Mock<ChannelWriter<ApiResultModel>> GetMockChannel()
        {
            var channelMock = new Mock<ChannelWriter<ApiResultModel>>();

            //channelMock.Setup(ch => ch.WriteAsync(It.IsAny<ApiResultModel>(), System.Threading.CancellationToken.None));

            return channelMock;
        }

        private Mock<ICacheHelper> GetMockForICacheHelper(Dictionary<string, decimal> priceCache2value)
        {
            var cacheMock = new Mock<ICacheHelper>();
            cacheMock.Setup(x => x.GetCityCodeForAirport(It.IsAny<string>()))
                     .ReturnsAsync((string airCode) => airport2CityMap[airCode]);

            cacheMock.Setup(x => x.GetAveragePrice(It.IsAny<string>()))
                     .ReturnsAsync((string key) => priceCache2value[key]);

            return cacheMock;
        }

        private Mock<IApiRequester> GetMockApiRequester(Dictionary<string, List<ApiResultModel>> results)
        {
            var mock = new Mock<IApiRequester>();

            mock.Setup(x => x.CreateRequest(It.IsAny<string>()))
                .Returns((string key) => 
                {
                    var desct = new Mock<IRequestDescriptor>();

                    desct.SetupGet(x => x.Key)
                          .Returns(key);

                    desct.Setup(x => x.AddParam(It.IsAny<IBaseSearchParam<It.IsAnyType>>()))
                         .Returns(desct.Object);

                    desct.Setup(x => x.Build())
                         .Returns(() =>
                        {
                            var sender = new Mock<IApiRequestSender>();
                            sender.SetupGet(x => x.Key).Returns(key);
                            sender.Setup(x => x.Send()).ReturnsAsync(() => results[key]);

                            return sender.Object;
                        });

                    return desct.Object;
                });

            return mock;
        }

        #endregion

        private void AssertSearchRes(OneResult act, ApiResultModel expected, decimal average)
        {
            Assert.True(expected.Price == act.Price, "Prices are not same");
            var expDisc = (expected.Price - average) / average * 100;
            var diff = Math.Abs(act.DiscountPercent - expDisc);

            Assert.True(diff < 0.05M, $"Discount value is wrong. Expexted: {expDisc} Actual: {act.DiscountPercent}");
        }

        private void AssertRefEquals<T>(T obj1, T obj2)
        {
            var eq = Object.ReferenceEquals(obj1, obj2);
            Assert.True(eq, "Objects are different");
        }
    }
}
