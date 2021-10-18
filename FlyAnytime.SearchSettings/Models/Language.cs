using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
using FlyAnytime.SearchSettings.MongoDb.Validation;
using FlyAnytime.Tools;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FlyAnytime.SearchSettings.Models
{
    public class Language : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class LanguageMap : RootEntityMap<Language>
    {
        public LanguageMap(IMongoDbContext context) : base(context, "Language")
        {
        }

        public override void SetCustomMapping(BsonClassMap<Language> classMap)
        {
            
        }

        public override void DoAfterMapping(BsonClassMap<Language> classMap)
        {
            AddUniqueIndex(x => x.Code);

            base.DoAfterMapping(classMap);
        }
    }

    public class LangugeValidator : Validator<Language>
    {
        public override bool Validate(Language entity, EntityErrorModel<Language> modelError)
        {
            var isValid = true;

            if (entity.Code.IsNullOrEmpty())
            {
                isValid = false;
                modelError.AddValidationError(x => x.Code, "Code is empty");
            }

            return isValid;
        }
    }
}
