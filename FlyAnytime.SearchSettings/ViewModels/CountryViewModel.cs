using AutoMapper;
using FlyAnytime.Core.AutoMapper;
using FlyAnytime.Core.Web.ViewModel;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.Tools;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.ViewModels
{
    public class CountryViewModel : BaseMongoVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public string DefSearchCurrencyCode { get; set; }

        public IEnumerable<LocalizationViewModel> Localizations { get; set; }
    }

    public class CountryProfile : BaseVmMongoProfile<Country, CountryViewModel>
    {
        protected override void MapDestination2Source(IMappingExpression<CountryViewModel, Country> mappingExpression)
        {
            base.MapDestination2Source(mappingExpression);
            mappingExpression.AfterMap((vm, ent) =>
            {
                if (ent.DefSearchCurrencyCode.IsNullOrEmpty())
                {
                    ent.DefSearchCurrencyCode = ent.CurrencyCode;
                }
            });
        }
    }
}
