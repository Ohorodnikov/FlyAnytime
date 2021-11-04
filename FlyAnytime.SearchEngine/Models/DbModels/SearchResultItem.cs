using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using FlyAnytime.Tools;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Models.DbModels
{
    public class SearchResultItem : Entity<long>
    {
        public string Code { get; set; }
        public decimal Price { get; set; }

        public long ArrivalToDestinationDateTimeUtc { get; set; }
        public long DepartureFromDestinationDateTimeUtc { get; set; }

        public long CreationDateTime { get; private set; } = DateTimeHelper.UnixNow;
    }

    public class SearchResultItemMapping : EntityMap<SearchResultItem, long>
    {
        public SearchResultItemMapping() : base("SRI") { }

        public override void SetMapping(EntityTypeBuilder<SearchResultItem> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
        }
    }
}
