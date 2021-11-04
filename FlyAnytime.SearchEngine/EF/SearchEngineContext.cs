using FlyAnytime.Core.EfContextBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.EF
{
    public class SearchEngineContext : BaseEfContext<SearchEngineContext>
    {
        public SearchEngineContext(DbContextOptions<SearchEngineContext> options, IServiceProvider serviceProvider) 
            : base(options, serviceProvider)
        {
        }
    }
}
