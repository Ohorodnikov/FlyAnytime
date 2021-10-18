using AutoMapper;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.Repository;
using FlyAnytime.SearchSettings.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Controllers
{
    public class CityController : SimpleCrudController<City, CityViewModel>
    {
        private readonly IMessageBus _messageBus;
        public CityController(
            IMessageBus messageBus,
            IRepository<City> repository,
            IMapper mapper
            ) :
            base(messageBus, repository, mapper)
        {
            _messageBus = messageBus;
        }

        protected override async Task OnSuccessCreate(City entity)
        {
            PublishCreateUpdateMessage(entity);
            await base.OnSuccessCreate(entity);
        }
        protected override async Task OnSuccessUpdate(City entity)
        {
            PublishCreateUpdateMessage(entity);
            await base.OnSuccessUpdate(entity);
        }

        private void PublishCreateUpdateMessage(City city)
        {
            var locals = new Dictionary<string, string>();
            foreach (var loc in city.Localizations)
                locals.Add(loc.Language.Code, loc.Value);
            
            var msg = new AddOrUpdateCityMessage(city.Code, city.Name, city.Country.Code, locals);
            _messageBus.Publish(msg);
        }

        protected override async Task OnSuccessDelete(City entity)
        {
            var msg = new DeleteCityMessage(entity.Code);
            _messageBus.Publish(msg);
            await base.OnSuccessDelete(entity);
        }
    }
}
