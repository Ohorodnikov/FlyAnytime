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

namespace FlyAnytime.SearchSettings.Models
{
    public class User : MongoRoot
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }

    public class UserMap : RootEntityMap<User>
    {
        public UserMap(IMongoDbContext context) : base(context, "User")
        {
        }

        public override void SetCustomMapping(BsonClassMap<User> classMap)
        {
        }

        public override void DoAfterMapping(BsonClassMap<User> classMap)
        {
            AddUniqueIndex(x => x.UserId);

            base.DoAfterMapping(classMap);
        }
    }

    public class UserValidator : Validator<User>
    {
        public override bool Validate(User entity, EntityErrorModel<User> modelError)
        {
            var isValid = true;

            if (entity.UserId.IsNullOrEmpty())
            {
                isValid = false;
                modelError.AddValidationError(x => x.UserId, "User Id must be filled");
            }

            return isValid;
        }
    }
}
