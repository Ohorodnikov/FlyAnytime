using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlyAnytime.SearchEngine.Models.DbModels
{
    public class Airport : Entity<long>
    {
        public string Code { get; set; }
        public City City { get; set; }
    }

    public class AirportMapping : EntityMap<Airport, long>
    {
        public AirportMapping() : base("Airport") { }

        public override void SetMapping(EntityTypeBuilder<Airport> mapBuilder)
        {
            mapBuilder.Property(x => x.Code).IsRequired();
        }
    }
}
