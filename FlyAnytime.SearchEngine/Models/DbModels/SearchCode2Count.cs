using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using FlyAnytime.Tools;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Models.DbModels
{
    public class SearchCode2Count : Entity<long>
    {
        public string Code { get; set; }
        public long SearchCount { get; set; }
        public long CreationDateTime { get; private set; } = DateTimeHelper.UnixNow;
    }

    public class SearchCode2CountMapping : EntityMap<SearchCode2Count, long>
    {
        public SearchCode2CountMapping() : base("SearchCount") { }

        public override void SetMapping(EntityTypeBuilder<SearchCode2Count> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
        }
    }
}
