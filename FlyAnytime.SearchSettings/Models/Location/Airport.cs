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
    public class Airport : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public ObjectId CityId { get; set; }
        public City City { get; set; }

        public IEnumerable<LocalizationItem> Localizations { get; set; }
    }

    public class AirportMap : RootEntityMap<Airport>
    {
        public AirportMap(IMongoDbContext context) : base(context, "Airport")
        {
        }

        public override void SetCustomMapping(BsonClassMap<Airport> classMap)
        {
            
        }
        public override void DoAfterMapping(BsonClassMap<Airport> classMap)
        {
            AddUniqueIndex(x => x.Code);

            base.DoAfterMapping(classMap);
        }
    }

    public class AirportValidator : Validator<Airport>
    {
        public override bool Validate(Airport entity, EntityErrorModel<Airport> modelError)
        {
            var isValid = true;

            if (entity.Code.IsNullOrEmpty())
            {
                isValid = false;
                modelError.AddValidationError(x => x.Code, "Code must be entered");
            }

            if (entity.CityId == ObjectId.Empty)
            {
                isValid = false;
                modelError.AddValidationError(x => x.Code, "City Id is required");
            }
            return isValid;
        }
    }
}
