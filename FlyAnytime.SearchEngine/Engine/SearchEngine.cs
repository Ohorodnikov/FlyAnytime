using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.Tools;
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.Engine
{
    public interface ISearchEngine
    {
        Task<IEnumerable<OneResult>> Search(MakeSearchMessage settings); 
    }

    public class SearchEngine : ISearchEngine
    {
        public async Task<IEnumerable<OneResult>> Search(MakeSearchMessage settings)
        {
            var res = new List<OneResult>();

            var oneRes1 = new OneResult
            {
                CityFrom = settings.CityFlyFrom,
                CityTo = "NY",
                DateTimeFrom = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(35)),
                DateTimeBack = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(40)),
                Price = 1488,
                DiscountPercent = 10,
                ResultUrl = ""
            };

            var oneRes2 = new OneResult
            {
                CityFrom = settings.CityFlyFrom,
                CityTo = "NY",
                DateTimeFrom = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(35)),
                DateTimeBack = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(40)),
                Price = 1488,
                DiscountPercent = 20,
                ResultUrl = ""
            };

            var oneRes3 = new OneResult
            {
                CityFrom = settings.CityFlyFrom,
                CityTo = "NY",
                DateTimeFrom = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(35)),
                DateTimeBack = DateTimeHelper.ToUtcUnix(DateTime.UtcNow.AddDays(40)),
                Price = 1488,
                DiscountPercent = 30,
                ResultUrl = ""
            };

            res.Add(oneRes1);
            res.Add(oneRes2);
            res.Add(oneRes3);

            return res;
        }
    }
}
