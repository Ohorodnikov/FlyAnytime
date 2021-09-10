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
    public class Language : MongoRoot
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class LanguageMap : RootEntityMap<Language>
    {
        public LanguageMap() : base("Language")
        {
        }

        public override void SetMapping(BsonClassMap<Language> classMap)
        {
        }
    }

    public class LangugeValidator : Validator<Language>
    {
        public override bool Validate(Language entity, EntityErrorModel modelError)
        {
            var isValid = true;

            if (entity.Code.IsNullOrEmpty())
            {
                isValid = false;
                modelError.AddValidationError<Language>(x => x.Code, "Code is empty");
            }

            return isValid;
        }
    }
}
