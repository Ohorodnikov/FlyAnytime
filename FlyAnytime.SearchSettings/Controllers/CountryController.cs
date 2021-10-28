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
    public class CountryController : SimpleCrudController<Country, CountryViewModel>
    {
        private readonly IMessageBus _messageBus;
        public CountryController(
            IMessageBus messageBus,
            IRepository<Country> languageRepository,
            IMapper mapper
            ) :
            base(messageBus, languageRepository, mapper)
        {
            _messageBus = messageBus;
        }

        protected override async Task OnSuccessCreate(Country entity)
        {
            PublishCreateUpdateMessage(entity);
            await base.OnSuccessCreate(entity);
        }
        protected override async Task OnSuccessUpdate(Country entity)
        {
            PublishCreateUpdateMessage(entity);
            await base.OnSuccessUpdate(entity);
        }

        private void PublishCreateUpdateMessage(Country country)
        {
            var locals = new Dictionary<string, string>();
            foreach (var loc in country.Localizations)
                locals.Add(loc.Language.Code, loc.Value);
            
            var msg = new AddOrUpdateCountryMessage(country.Code, country.Name, country.DefSearchCurrencyCode, locals);
            _messageBus.Publish(msg);
        }

        protected override async Task OnSuccessDelete(Country entity)
        {
            var msg = new DeleteCountryMessage(entity.Code);
            _messageBus.Publish(msg);
            await base.OnSuccessDelete(entity);
        }
    }
}
