using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi
{
    public abstract partial class KiwiApiRequester : IApiRequester
    {
        private readonly string server = "https://tequila-api.kiwi.com/v2";

        private readonly List<OneRequestInfo> requests = new List<OneRequestInfo>();

        public IReadOnlyCollection<IApiRequestSender> Requests => requests.AsReadOnly();

        protected abstract string GetApiKey();
     
        protected abstract string Type { get; }

        public IRequestDescriptor CreateRequest(string key)
        {
            var oneRequest = new OneRequestInfo(key, $"{server}/{Type}");

            oneRequest.SetApiKey(GetApiKey());

            return oneRequest;
        }
    }
}
