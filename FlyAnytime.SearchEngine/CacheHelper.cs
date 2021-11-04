using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Engine;
using FlyAnytime.SearchEngine.Models.DbModels;
using FlyAnytime.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine
{
    public interface ICacheHelper
    {
        Task<decimal> GetAveragePrice(string directionKey);
        Task<string> GetCityCodeForAirport(string airportCode);
    }

    public class CacheHelper : ICacheHelper
    {
        private readonly IMemoryCache _cache;
        private readonly SearchEngineContext _context;
        public CacheHelper(IMemoryCache cache, SearchEngineContext context)
        {
            _cache = cache;
            _context = context;
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

        public async Task<decimal> GetAveragePrice(string directionKey)
        {
            return await _cache.GetOrCreateAsync(directionKey, async item =>
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
    }
}
