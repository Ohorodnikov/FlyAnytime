using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Models.SearchSettings
{
    public class ChatSearchGroup : MongoInternalEntity
    {
        public string Code { get; set; }

        public IEnumerable<ObjectId> AirportsIds { get; set; }
        public IEnumerable<Airport> Airports { get; set; }


        //public IEnumerable<ObjectId> CitiesIds { get; set; }
        //public IEnumerable<City> Cities { get; set; }


        //public IEnumerable<ObjectId> CountriesIds { get; set; }
        //public IEnumerable<Country> Countries { get; set; }

        public bool IsActive { get; set; }
    }

    public class ChatSearchGroupMap : InternalEntityMap<ChatSearchGroup>
    {
        public override void SetCustomMapping(BsonClassMap<ChatSearchGroup> classMap)
        {
        }
    }

    public class ChatSearchGroupValidator : Validator<ChatSearchGroup>
    {
        public override bool Validate(ChatSearchGroup entity, EntityErrorModel<ChatSearchGroup> modelError)
        {
            var isValid = true;

            if (entity.AirportsIds == null || entity.AirportsIds.Any())
            {
                isValid = false;
                modelError.AddValidationError(x => x.AirportsIds, "Airports ids must be filled!");
            }

            return isValid;
        }
    }
}
