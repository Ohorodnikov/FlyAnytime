using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.SearchEngine.Engine.ApiRequesters;
using FlyAnytime.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine
{
    public static class FilterHelper
    {
        public static Func<ApiResultModel, bool> GetFilterForPrice(this MakeSearchMessage msg, decimal averagePrice)
        {
            var mode = msg.PriceSettings.Type;
            var amount = msg.PriceSettings.Amount;
            var is0 = averagePrice == 0;
            Func<ApiResultModel, bool> priceFlt = mode switch
            {
                SearchPriceSettingsType.FixPrice => r => r.Price <= amount,
                SearchPriceSettingsType.PercentDiscount => r => is0 || (r.Price - averagePrice) / averagePrice >= amount,
                _ => throw new NotImplementedException(),
            };

            return priceFlt;
        }

        public static bool IsDateInsideAllowedSlots(long dateTime, Dictionary<Days, HashSet<byte>> allowedSlots)
        {
            var dt = DateTimeHelper.UnixToUtc(dateTime);

            var day = dt.DayOfWeek;
            var d = day switch
            {
                DayOfWeek.Sunday => Days.Sunday,
                DayOfWeek.Monday => Days.Monday,
                DayOfWeek.Tuesday => Days.Tuesday,
                DayOfWeek.Wednesday => Days.Wednesday,
                DayOfWeek.Thursday => Days.Thursday,
                DayOfWeek.Friday => Days.Friday,
                DayOfWeek.Saturday => Days.Saturday,
                _ => throw new NotImplementedException(),
            };

            return allowedSlots.TryGetValue(d, out var daySlots) 
                   && daySlots.Contains((byte)dt.Hour);
        }
    }
}
