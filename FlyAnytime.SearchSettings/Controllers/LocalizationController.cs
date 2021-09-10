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
    public class LocalizationController : Controller
    {
        private readonly IMessageBus _messageBus;
        private readonly IRepository<Language> _languageRepository;
        public LocalizationController(
            IMessageBus messageBus,
            IRepository<Language> languageRepository
            )
        {
            _messageBus = messageBus;
            _languageRepository = languageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetLanguage(string id)
        {
            if (!ObjectId.TryParse(id, out var objId))
                return Json(new { success = false, message = "Id is wrong" });

            var lang = await _languageRepository.GetById(objId);

            var vm = new LanguageViewModel
            {
                Id = lang.Id.ToString(),
                Code = lang.Code,
                Name = lang.Name
            };

            return Json(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewLanguage(LanguageViewModel languageViewModel)
        {
            var lang = new Language
            {
                Code = languageViewModel.Code,
                Name = languageViewModel.Name
            };

            var (success, errorModel) = await _languageRepository.TryCreate(lang);
            if (success)
            {
                var newLanguageMessage = new AddNewLanguageMessage(lang.Code, lang.Name);
                _messageBus.Publish(newLanguageMessage);

                return Json(new { success = true });

                //return success
            }
            else
            {
                //return validation error
                return Json(new { success = true , errorModel = errorModel });
            }
        }
    }
}
