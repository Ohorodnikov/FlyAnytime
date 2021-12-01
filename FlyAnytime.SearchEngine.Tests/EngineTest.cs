using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.SearchEngine.EF;
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

        private static DateTime monMorning = new DateTime(2021, 12, 20, 4, 34, 0, DateTimeKind.Utc);
        private static DateTime monDay = new DateTime(2021, 12, 20, 11, 34, 0, DateTimeKind.Utc);
        private static DateTime monEvening = new DateTime(2021, 12, 20, 17, 34, 0, DateTimeKind.Utc);

        private static DateTime tueMorning = monMorning.AddDays(1);
        private static DateTime tueDay = monDay.AddDays(1);
        private static DateTime tueEvening = monEvening.AddDays(1);

        private static DateTime wenMorning = monMorning.AddDays(2);
        private static DateTime wenDay = monDay.AddDays(2);
        private static DateTime wenEvening = monEvening.AddDays(2);

        private static DateTime thurMorning = monMorning.AddDays(3);
        private static DateTime thurDay = monDay.AddDays(3);
        private static DateTime thur5PM = monEvening.AddDays(3);

        private static DateTime frdMorning = monMorning.AddDays(4);
        private static DateTime frdDay = monDay.AddDays(4);
        private static DateTime frdEvening = monEvening.AddDays(4);

        private static DateTime satMorning = monMorning.AddDays(5);
        private static DateTime satDay = monDay.AddDays(5);
        private static DateTime satEvening = monEvening.AddDays(5);

        private static DateTime sunMorning = monMorning.AddDays(6);
        private static DateTime sunDay = monDay.AddDays(6);
        private static DateTime sunEvening = monEvening.AddDays(6);

        private static HashSet<byte> allDay = new HashSet<byte> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        private static HashSet<byte> earlyMorning = new HashSet<byte> { 0, 1, 2, 3, 4, 5, 6 };
        private static HashSet<byte> evening = new HashSet<byte> { 17, 18, 19, 20, 21, 22, 23 };
        private static HashSet<byte> midDay = new HashSet<byte> { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        #endregion

        [Fact]
        public async Task SearEn_2Airp2City_FixPrice_AllSlots_Test()
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
                CreateRequstResult(400, USD, NY, frdDay, tueMorning),
                CreateRequstResult(450, USD, NY, thur5PM, sunDay),
                CreateRequstResult(460, USD, NY, satEvening, wenEvening),
                CreateRequstResult(470, USD, NY, frdMorning, tueMorning),
                CreateRequstResult(475, USD, NY, monDay, frdMorning),
                CreateRequstResult(500, USD, NY, frdMorning, tueMorning),
                CreateRequstResult(600, USD, NY, frdMorning, tueMorning),
            };

            var result2Lon = new List<ApiResultModel>
            {
                CreateRequstResult(15, USD, London, frdDay, tueMorning),
                CreateRequstResult(20, USD, London, thur5PM, sunDay),
                CreateRequstResult(25, USD, London, satEvening, wenEvening),
                CreateRequstResult(30, USD, London, monDay, frdMorning),
                CreateRequstResult(35, USD, London, frdMorning, tueMorning),
            };

            var res = new Dictionary<string, List<ApiResultModel>>
            {
                { CacheKey_IEV2LON, result2Lon },
                { CacheKey_IEV2NY, result2NY },
            };

            var apiMock = GetMockApiRequester(res);
            var cacheMock = GetMockForICacheHelper(priceCache2value);
            var channelMock = GetMockChannel();
            var dbContextMock = GetMockForContext();

            var engine = new Engine.SearchEngine(apiMock.Object, cacheMock.Object, channelMock.Object, dbContextMock.Object);
            var results = await engine.Search(searchParams);

            channelMock.Verify(x => x.WriteAsync(It.IsAny<List<ApiResultModel>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
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
        public async Task SearEn_2Airp2City_FixPrice_FilterBySlots_Test()
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
                CreateRequstResult(400, USD, NY, frdDay, tueMorning), //-
                CreateRequstResult(450, USD, NY, thur5PM, sunDay), //+
                CreateRequstResult(460, USD, NY, satEvening, wenEvening), //-
                CreateRequstResult(470, USD, NY, frdMorning, tueMorning),//+
                CreateRequstResult(475, USD, NY, monDay, frdMorning),//-
                CreateRequstResult(500, USD, NY, frdMorning, tueMorning),//+
                CreateRequstResult(600, USD, NY, frdMorning, tueMorning),//-
            };

            var result2Lon = new List<ApiResultModel>
            {
                CreateRequstResult(15, USD, London, frdDay, tueMorning),//-
                CreateRequstResult(20, USD, London, thur5PM, sunDay),//+
                CreateRequstResult(25, USD, London, satEvening, wenEvening),//-
                CreateRequstResult(30, USD, London, monDay, frdMorning),//-
                CreateRequstResult(35, USD, London, frdMorning, tueMorning),//+
            };

            var res = new Dictionary<string, List<ApiResultModel>>
            {
                { CacheKey_IEV2LON, result2Lon },
                { CacheKey_IEV2NY, result2NY },
            };

            var apiMock = GetMockApiRequester(res);
            var cacheMock = GetMockForICacheHelper(priceCache2value);
            var channelMock = GetMockChannel();
            var dbContextMock = GetMockForContext();

            var engine = new Engine.SearchEngine(apiMock.Object, cacheMock.Object, channelMock.Object, dbContextMock.Object);
            var results = await engine.Search(searchParams);

            channelMock.Verify(x => x.WriteAsync(It.IsAny<List<ApiResultModel>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
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
        public async Task SearEn_2Airp2City_FixPrice_AllSlots_FilterByMaxPrice_Test()
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
                CreateRequstResult(450, USD, NY, frdMorning, tueMorning),
                CreateRequstResult(475, USD, NY, monDay, frdMorning),
                CreateRequstResult(510, USD, NY, frdMorning, tueMorning),
                CreateRequstResult(600, USD, NY, frdMorning, tueMorning),
            };

            var result2Lon = new List<ApiResultModel>
            {
                CreateRequstResult(15, USD, London, frdDay, tueMorning),
                CreateRequstResult(20, USD, London, thur5PM, sunDay),
                CreateRequstResult(25, USD, London, satEvening, wenEvening),
                CreateRequstResult(30, USD, London, monDay, frdMorning),
                CreateRequstResult(35, USD, London, frdMorning, tueMorning),
            };

            var res = new Dictionary<string, List<ApiResultModel>>
            {
                { CacheKey_IEV2LON, result2Lon },
                { CacheKey_IEV2NY, result2NY },
            };

            var apiMock = GetMockApiRequester(res);
            var cacheMock = GetMockForICacheHelper(priceCache2value);
            var channelMock = GetMockChannel();
            var dbContextMock = GetMockForContext();

            var engine = new Engine.SearchEngine(apiMock.Object, cacheMock.Object, channelMock.Object, dbContextMock.Object);
            var results = await engine.Search(searchParams);

            channelMock.Verify(x => x.WriteAsync(It.IsAny<List<ApiResultModel>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
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
                FromDateTime = new FlyDateTimeInfo
                {
                    StartLocal = departure.ToUtcUnix()
                },
                ReturnDateTime = new FlyDateTimeInfo
                {
                    EndLocal = arrival.ToUtcUnix()
                },
                CityCodeTo = flyTo
            };
        }

        #region Moq

        private Mock<SearchEngineContext> GetMockForContext()
        {
            var mock = new Mock<SearchEngineContext>();

            return mock;
        }

        private Mock<ChannelWriter<List<ApiResultModel>>> GetMockChannel()
        {
            var channelMock = new Mock<ChannelWriter<List<ApiResultModel>>>();

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
