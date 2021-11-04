using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models;
using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels;
using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.ResponseModels;
using FlyAnytime.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi
{
    internal class OneRequestInfo : IRequestDescriptor, IApiRequestSender
    {
        public OneRequestInfo(string id, string server)
        {
            Key = id;
            HttpClient = new HttpClient();
            Server = server;
        }

        public string Key { get; }
        public HttpClient HttpClient { get; }
        public string Server { get; }
        public Uri Uri { get; private set; }

        private HttpResponseMessage ResponseMessage { get; set; }

        private readonly List<IBaseSearchParam> searchParams = new List<IBaseSearchParam>();

        public IReadOnlyCollection<IBaseSearchParam> SearchParams => searchParams.AsReadOnly();

        public void SetApiKey(string key) => HttpClient.DefaultRequestHeaders.Add("apikey", key);

        private bool IsRoundSearch()
        {
            var roundSearchParamsType = new[]
            {
                typeof(ReturnDateFrom),
                typeof(ReturnDateTo),
                typeof(NightsInDestFrom),
                typeof(NightsInDestTo),
            };

            return searchParams.Any(x => roundSearchParamsType.Contains(x.GetType()));
        }

        IRequestDescriptor IRequestDescriptor.AddParam<TValue>(IBaseSearchParam<TValue> searchParam)
        {
            searchParams.Add(searchParam);

            return this;
        }

        IApiRequestSender IRequestDescriptor.Build()
        {
            var query = string.Join("&", SearchParams.Select(x => $"{x.ParamName}={x.ToSearchString()}"));
            Uri = new Uri($"{Server}?{query}");

            return this;
        }

        async Task<List<ApiResultModel>> IApiRequestSender.Send()
        {
            ResponseMessage ??= await HttpClient.GetAsync(Uri);

            if (!ResponseMessage.IsSuccessStatusCode)
                return new List<ApiResultModel>();

            var retCont = await ResponseMessage.Content.ReadAsStringAsync();

            var resultTyped = JsonConvert.DeserializeObject<SearchResult>(retCont);

            var isRoundSearch = IsRoundSearch();

            var data = new List<ApiResultModel>();

            foreach (var oneRes in resultTyped.Results)
            {
                var routeOrdered = oneRes.Route.OrderBy(x => DateTimeHelper.IsoToUnix(x.UtcArrival));

                var firstDeparture = routeOrdered.Where(x => x.IsReturn == 0).First().LocalDeparture;
                var arrivalToDestination = routeOrdered.Where(x => x.IsReturn == 0).Last().UtcArrival;

                var isRet = isRoundSearch ? 1 : 0;

                var lastArrival = routeOrdered.Where(x => x.IsReturn == isRet).Last().LocalDeparture;
                var departureFromDestination = routeOrdered.Where(x => x.IsReturn == isRet).First().UtcDeparture;

                var apiModel = new ApiResultModel
                {
                    CityCodeFrom = oneRes.CityCodeFrom,
                    CityCodeTo = oneRes.CityCodeTo,

                    Price = oneRes.Price,
                    PriceInEur = oneRes.Price / resultTyped.FxRate,
                    Currency = resultTyped.Currency,

                    DepartureFromDateTimeLocal = DateTimeHelper.IsoToUnix(firstDeparture),
                    ArrivalBackDateTimeLocal = DateTimeHelper.IsoToUnix(lastArrival),

                    ArrivalDateTimeToDestinationUtc = DateTimeHelper.IsoToUnix(arrivalToDestination),
                    BackDateTimeFromDestinationUtc = DateTimeHelper.IsoToUnix(departureFromDestination),

                    StepoverCountTo = routeOrdered.Where(x => x.IsReturn == 0).Count() - 1,
                    StepoverCountBack = isRoundSearch ? routeOrdered.Where(x => x.IsReturn == 1).Count() - 1 : 0,

                    LinkOnResult = oneRes.Link
                };

                data.Add(apiModel);
            }

            return data;
        }
    }
}
