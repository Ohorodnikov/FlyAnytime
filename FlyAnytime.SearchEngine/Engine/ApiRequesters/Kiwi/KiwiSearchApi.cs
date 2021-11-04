using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi
{
    public class KiwiSearchApi : KiwiApiRequester
    {
        IConfiguration Configuration { get; }
        KiwiConfig KiwiConfig { get; }

        protected override string Type => "search";

        public KiwiSearchApi(IConfiguration configuration) : base()
        {
            Configuration = configuration;
            KiwiConfig = Configuration.GetSection("KiwiApiKeys").Get<KiwiConfig>();
        }

        protected override string GetApiKey()
        {
            return KiwiConfig.Search;
        }
    }
}
