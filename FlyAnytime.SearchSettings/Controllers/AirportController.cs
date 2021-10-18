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
    public class AirportController : SimpleCrudController<Airport, AirportViewModel>
    {
        private readonly IMessageBus _messageBus;
        public AirportController(
            IMessageBus messageBus,
            IRepository<Airport> repository,
            IMapper mapper
            ) :
            base(messageBus, repository, mapper)
        {
            _messageBus = messageBus;
        }

        protected override async Task OnSuccessCreate(Airport entity)
        {
            PublishCreateUpdateMessage(entity);
            await base.OnSuccessCreate(entity);
        }
        protected override async Task OnSuccessUpdate(Airport entity)
        {
            PublishCreateUpdateMessage(entity);
            await base.OnSuccessUpdate(entity);
        }

        private void PublishCreateUpdateMessage(Airport airport)
        {
            var locals = new Dictionary<string, string>();
            foreach (var loc in airport.Localizations)
                locals.Add(loc.Language.Code, loc.Value);
            
            var msg = new AddOrUpdateAirportMessage(airport.Code, airport.Name, airport.City.Code, locals);
            _messageBus.Publish(msg);
        }

        protected override async Task OnSuccessDelete(Airport entity)
        {
            var msg = new DeleteAirportMessage(entity.Code);
            _messageBus.Publish(msg);
            await base.OnSuccessDelete(entity);
        }
    }
}
