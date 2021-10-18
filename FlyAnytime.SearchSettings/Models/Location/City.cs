using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using FlyAnytime.Tools;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyAnytime.SearchSettings.Models.Location
{
    public class City : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public ObjectId CountryId { get; set; }
        public Country Country { get; set; }

        public IEnumerable<LocalizationItem> Localizations { get; set; }
    }

    public class CityMap : RootEntityMap<City>
    {
        public CityMap(IMongoDbContext context) : base(context, "City")
        {
        }

        public override void SetCustomMapping(BsonClassMap<City> classMap)
        {

        }
        public override void DoAfterMapping(BsonClassMap<City> classMap)
        {
            AddUniqueIndex(x => x.Code);

            base.DoAfterMapping(classMap);
        }
    }

    public class CityValidator : Validator<City>
    {
        public override bool Validate(City entity, EntityErrorModel<City> modelError)
        {
            var isValid = true;

            if (entity.Code.IsNullOrEmpty())
            {
                isValid = false;
                modelError.AddValidationError(x => x.Code, "Code must be entered");
            }

            if (entity.CountryId == ObjectId.Empty)
            {
                isValid = false;
                modelError.AddValidationError(x => x.Code, "Country Id is required");
            }

            return isValid;
        }
    }
}
