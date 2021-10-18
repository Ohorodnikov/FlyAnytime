using AutoMapper;
using FlyAnytime.SearchSettings.Models;
using MongoDB.Bson;

namespace FlyAnytime.SearchSettings.ViewModels
{
    public class LocalizationViewModel : BaseMongoVm
    {
        public string Value { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageId { get; set; }
    }

    public class LocalizationProfile : BaseVmMongoProfile<LocalizationItem, LocalizationViewModel>
    {
        protected override void MapSource2Destination(IMappingExpression<LocalizationItem, LocalizationViewModel> mappingExpression)
        {
            mappingExpression
                .ForMember(x => x.LanguageId, opt => opt.MapFrom(x => x.LanguageId.ToString()))
                .ForMember(x => x.LanguageCode, opt => opt.MapFrom((e, vm) => e.Language?.Code ?? ""))
                ;

            base.MapSource2Destination(mappingExpression);
        }
    }
}
