using AutoMapper;
using FlyAnytime.Core.AutoMapper;
using FlyAnytime.Core.Web.ViewModel;
using FlyAnytime.SearchSettings.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.ViewModels
{
    public class LanguageViewModel : BaseMongoVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Culture { get; set; }
    }

    public class LanguageProfile : BaseVmMongoProfile<Language, LanguageViewModel>
    {
        
    }
}
