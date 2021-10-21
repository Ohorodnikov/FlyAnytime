using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using FlyAnytime.Tools;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Models.Location
{
    public class Country : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string CurrencyCode { get; set; }
        public string DefSearchCurrencyCode { get; set; }

        public IEnumerable<LocalizationItem> Localizations { get; set; }
    }

    public class CountryMap : RootEntityMap<Country>
    {
        public CountryMap(IMongoDbContext context) : base(context, "Country")
        {
        }

        public override void SetCustomMapping(BsonClassMap<Country> classMap)
        {

        }

        public override void DoAfterMapping(BsonClassMap<Country> classMap)
        {
            AddUniqueIndex(x => x.Code);

            base.DoAfterMapping(classMap);
        }
    }

    public class CountryValidator : Validator<Country>
    {
        public override bool Validate(Country entity, EntityErrorModel<Country> modelError)
        {
            var isValid = true;

            if (entity.Code.IsNullOrEmpty())
            {
                isValid = false;
                modelError.AddValidationError(x => x.Code, "Code must be entered");
            }

            return isValid;
        }
    }
}
