using AutoMapper;
using FlyAnytime.Core.AutoMapper;
using FlyAnytime.Core.Web.ViewModel;
using FlyAnytime.SearchSettings.Models.Location;
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

        public IEnumerable<LocalizationViewModel> Localizations { get; set; }
    }

    public class CountryProfile : BaseVmMongoProfile<Country, CountryViewModel>
    {
        
    }
}
