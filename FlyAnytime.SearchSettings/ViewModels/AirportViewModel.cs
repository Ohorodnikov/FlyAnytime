using FlyAnytime.Core.Web.ViewModel;
using FlyAnytime.SearchSettings.Models.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.ViewModels
{
    public class AirportViewModel : BaseMongoVm
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string CityId { get; set; }

        public IEnumerable<LocalizationViewModel> Localizations { get; set; }
    }

    public class AirportProfile : BaseVmMongoProfile<Airport, AirportViewModel>
    {

    }
}
