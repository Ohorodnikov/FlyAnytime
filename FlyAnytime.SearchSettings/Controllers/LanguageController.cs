using AutoMapper;
using FlyAnytime.Core.Web;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.Repository;
using FlyAnytime.SearchSettings.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Controllers
{
    public class LanguageController : SimpleCrudController<Language, LanguageViewModel>
    {
        private readonly IMessageBus _messageBus;
        public LanguageController(
            IMessageBus messageBus,
            IRepository<Language> languageRepository,
            IMapper mapper
            ) 
            : base(messageBus, languageRepository, mapper)
        {
            _messageBus = messageBus;
        }

        protected override async Task OnSuccessCreate(Language entity)
        {
            var newLanguageMessage = new AddNewLanguageMessage(entity.Code, entity.Name, entity.Culture);
            _messageBus.Publish(newLanguageMessage);

            await Task.CompletedTask;
        }

        protected override Task OnSuccessUpdate(Language entity)
        {
            return base.OnSuccessUpdate(entity);
        }

        protected override Task OnSuccessDelete(Language entity)
        {
            return base.OnSuccessDelete(entity);
        }
    }
}
