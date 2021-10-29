using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using FlyAnytime.Core.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Scheduler.Models
{
    public abstract class BaseSearchJobData : Entity<long>
    {
        public long ChatId { get; set; }
        public Guid SettingsId { get; set; }
        public string CityFlyFrom { get; set; }

        /// <summary>
        /// Must be stored as KBP;IEV; and so on
        /// </summary>
        public string AirportsFlyTo { get; set; }

        public string Currency { get; set; }
        public decimal PriceAmount { get; set; }
        public SearchPriceSettingsType PriceType { get; set; }

        public int TripDaysCountMin { get; set; }
        public int TripDaysCountMax { get; set; }
    }

    public abstract class BaseJobDataMap<TJobData> : EntityMap<TJobData, long>
        where TJobData : BaseSearchJobData
    {
        public BaseJobDataMap(string tableName) : base(tableName)
        {
        }

        public override void SetMapping(EntityTypeBuilder<TJobData> mapBuilder)
        {
            mapBuilder.Property(x => x.ChatId).IsRequired();
            mapBuilder.Property(x => x.SettingsId).IsRequired();
            //mapBuilder.Property(x => x.AirportsFlyTo).IsRequired();
            //mapBuilder.Property(x => x.CityFlyFrom).IsRequired();
            //mapBuilder.Property(x => x.Currency).IsRequired();
        }
    }
}
