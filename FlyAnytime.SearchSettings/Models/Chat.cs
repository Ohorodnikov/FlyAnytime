using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.Models.SearchSettings;
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

namespace FlyAnytime.SearchSettings.Models
{
    public class Chat : MongoRoot
    {
        public long ChatId { get; set; }

        public ObjectId ChatOwnerId { get; set; }
        public User ChatOwner { get; set; }

        public bool IsGroup { get; set; }
        public string Title { get; set; }

        public IEnumerable<ChatSearchSettings> SearchSettings { get; set; }

        public ObjectId FlyFromId { get; set; }
        public City FlyFrom { get; set; }
    }

    public class ChatMap : RootEntityMap<Chat>
    {
        public ChatMap(IMongoDbContext context) : base(context, "Chat")
        {
        }

        public override void SetCustomMapping(BsonClassMap<Chat> classMap)
        {
        }

        public override void DoAfterMapping(BsonClassMap<Chat> classMap)
        {
            AddUniqueIndex(x => x.ChatId);

            base.DoAfterMapping(classMap);
        }
    }

    public class ChatValidator : Validator<Chat>
    {
        public override bool Validate(Chat entity, EntityErrorModel<Chat> modelError)
        {
            var isValid = true;

            if (entity.ChatId == default)
            {
                isValid = false;
                modelError.AddValidationError(x => x.ChatId, "Chat Id must be filled");
            }

            if (entity.ChatOwnerId == ObjectId.Empty)
            {
                isValid = false;
                modelError.AddValidationError(x => x.ChatOwnerId, "User Id must be filled");
            }

            return isValid;
        }
    }
}
