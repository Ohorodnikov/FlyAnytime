using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    public static class FlyDateRangeHelper
    {
        public static IRequestDescriptor AddFlyFromAndReturnDates(this IRequestDescriptor setter, DateTime dateFrom, DateTime returnTo)
        {
            if (returnTo < dateFrom)
                throw new ArgumentOutOfRangeException($"{nameof(returnTo)} must be greater than {nameof(dateFrom)}");

            setter.AddParam(new DateFrom(dateFrom));
            setter.AddParam(new ReturnDateFrom(dateFrom));

            setter.AddParam(new DateTo(returnTo));
            setter.AddParam(new ReturnDateTo(returnTo));

            return setter;
        }

        public static IRequestDescriptor AddFlyFromAndReturnDates(this IRequestDescriptor setter, DateTime dateFrom, DateTime returnTo, int nightsMin, int nightsMax)
        {
            if (returnTo < dateFrom)
                throw new ArgumentOutOfRangeException($"{nameof(returnTo)} must be greater than {nameof(dateFrom)}");

            setter.AddNightsInDestination(nightsMin, nightsMax);

            setter.AddParam(new DateFrom(dateFrom));
            setter.AddParam(new ReturnDateFrom(dateFrom));

            setter.AddParam(new DateTo(returnTo));
            setter.AddParam(new ReturnDateTo(returnTo));

            return setter;
        }
    }
}
