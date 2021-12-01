using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Engine;
using FlyAnytime.SearchEngine.Engine.ApiRequesters;
using FlyAnytime.SearchEngine.Models.DbModels;
using FlyAnytime.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine
{
    public interface ICacheHelper
    {
        Task<decimal> GetAveragePrice(string directionKey);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directionCode"></param>
        /// <param name="mostPopularPercent"></param>
        /// <returns>Returns first value with chance to get is more than <paramref name="mostPopularPercent"/></returns>
        Task<decimal> GetSmallestMostPopularPrice(string directionCode, byte mostPopularPercent);
        Task<string> GetCityCodeForAirport(string airportCode);
        Task<List<TSearchRes>> GetOrAddSearchResults<TSearchRes>(string directionKey, long flyFromDT, long flyToDT, Func<Task<List<TSearchRes>>> resGetter)
            where TSearchRes : ApiResultModel;
    }

    public class CacheHelper : ICacheHelper
    {
        private readonly IMemoryCache _cache;
        private readonly SearchEngineContext _context;
        private readonly ILogger<CacheHelper> _logger;

        public CacheHelper(IMemoryCache cache, SearchEngineContext context, ILogger<CacheHelper> logger)
        {
            _cache = cache;
            _context = context;
            _logger = logger;
        }
        
        public async Task<List<TSearchRes>> GetOrAddSearchResults<TSearchRes>(string directionKey,
                                                                              long flyFromDT,
                                                                              long flyToDT,
                                                                              Func<Task<List<TSearchRes>>> resGetter)
            where TSearchRes : ApiResultModel
        {
            var masterKeyPref = "dir2diapason";

            var masterKey = GenerateKey(masterKeyPref, directionKey);

            var q = _cache.GetOrCreate(masterKey, e => 
            {
                e.SetSlidingExpiration(TimeSpan.FromHours(24));

                return new List<(long from, long to)>();
            });

            foreach (var key in q)
            {
                var flyFrom = key.from;
                var flyTo = key.to;

                if (flyFrom <= flyFromDT && flyTo >= flyToDT)
                {
                    if (_cache.TryGetValue((directionKey, flyFrom, flyTo), out var res1))
                    {
                        _logger.LogInformation($"Read from cache for key: {masterKey}");
                        var r = (List<TSearchRes>)res1;

                        return r.Copy().Where(x => x.FromDateTime.StartUtc >= flyFromDT && x.ReturnDateTime.EndUtc <= flyToDT).ToList();
                    }
                }
            }

            q.Add((flyFromDT, flyToDT));

            PostEvictionDelegate onSubKeyExpire = (object key, object value, EvictionReason reason, object st) =>
            {
                var keyTyped = ((string direction, long from, long to))key;
                var masterKey2 = GenerateKey(masterKeyPref, keyTyped.direction);

                if (_cache.TryGetValue(masterKey2, out var res))
                {
                    var rTyped = (List<(long, long)>)res;

                    rTyped.Remove((keyTyped.from, keyTyped.to));
                }
            };

            var expireTime = TimeSpan.FromMinutes(20);
            var exToken = new CancellationChangeToken(new CancellationTokenSource(expireTime).Token);
            var subKey = (directionKey, flyFromDT, flyToDT);

            var res = await _cache.GetOrCreateAsync(subKey, async item =>
            {
                _logger.LogInformation($"Send search request for key: {masterKey}");

                item
                    .SetAbsoluteExpiration(expireTime)
                    .AddExpirationToken(exToken)
                    .RegisterPostEvictionCallback(onSubKeyExpire);

                return await resGetter();
            });

            return res.Copy();
        }

        private static readonly ConcurrentDictionary<string, string> airport2city = new ConcurrentDictionary<string, string>();
        public async Task<string> GetCityCodeForAirport(string airportCode)
        {
            if (airport2city.TryGetValue(airportCode, out var cityCode))
                return cityCode;

            var val = await _context.Set<Airport>()
                                        .Where(x => x.Code == airportCode)
                                        .Select(x => x.City.Code)
                                        .FirstOrDefaultAsync();

            if (val == null)
                return null;

            return airport2city.GetOrAdd(airportCode, val);
        }

        private string GenerateKey(params string[] parts)
        {
            return string.Join("-!-", parts);
        }

        private string[] ParseKey(string key)
        {
            return key.Split("-!-");
        }

        public async Task<decimal> GetAveragePrice(string directionKey)
        {
            return await _cache.GetOrCreateAsync("av-" + directionKey, async item =>
            {
                var key = item.Key.ToString();
                var now = DateTimeHelper.UnixNow;
                var historyPeriod = DateTime.Now.AddMonths(-6).ToUtcUnix();
                var priceHistory = await _context.Set<SearchResultItem>()
                                            .Where(i => i.Code == key && i.CreationDateTime >= historyPeriod)
                                            .Select(x => x.Price)
                                            .ToListAsync();

                item.SetSlidingExpiration(TimeSpan.FromHours(8));

                return AverageCalculator.GetAvarege(priceHistory, 0.8M);
            });
        }

        public async Task<decimal> GetSmallestMostPopularPrice(string directionKey, byte mostPopularPercent)
        {
            const string prefix = "qua";
            var key = GenerateKey(prefix, directionKey, mostPopularPercent.ToString());
            return await _cache.GetOrCreateAsync(key, async item =>
            {
                var keyParts = ParseKey(item.Key.ToString());
                var key = keyParts[1];
                var now = DateTimeHelper.UnixNow;
                var historyPeriod = DateTime.Now.AddMonths(-6).ToUtcUnix();

                var priceHistory = await _context.Set<SearchResultItem>()
                                            .Where(i => i.Code == key && i.CreationDateTime >= historyPeriod)
                                            .Select(x => x.Price)
                                            .ToListAsync();

                var searchesCountTask = _context.Set<SearchCode2Count>()
                                            .Where(i => i.Code == key && i.CreationDateTime >= historyPeriod)
                                            .Select(x => x.SearchCount)
                                            .FirstOrDefaultAsync();

                var pricesAggregated = AverageCalculator.AggregatePrices(priceHistory);
                var searchesCount = await searchesCountTask;

                if (searchesCount == default || pricesAggregated == null || pricesAggregated.Count == 0)
                {
                    item.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
                    return 0;
                }

                var arrOrderd = pricesAggregated.OrderBy(x => x.Value);

                item.SetAbsoluteExpiration(TimeSpan.FromHours(8));
                var percentValue = int.Parse(keyParts[2]);
                foreach (var priceStat in arrOrderd)
                {
                    var chancePercent = priceStat.Count*100 / searchesCount;
                    if (chancePercent >= percentValue)
                        return priceStat.Value;
                }
                    
                return arrOrderd.Last().Value;
            });
        }
    }
}
