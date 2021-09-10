using FlyAnytime.Core.Entity;
using FlyAnytime.SearchSettings.Models.SearchSettings;
using FlyAnytime.SearchSettings.MongoDb;
using FlyAnytime.SearchSettings.MongoDb.Mapping;
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
        public string ChatId { get; set; }

        public ObjectId ChatOwnerId { get; set; }
        public User ChatOwner { get; set; }

        public bool IsGroup { get; set; }
        public string Title { get; set; }

        public IEnumerable<ChatSearchSettings> SearchSettings { get; set; }
    }

    public class ChatMap : RootEntityMap<Chat>
    {
        public ChatMap() : base("Chat")
        {
        }

        public override void SetMapping(BsonClassMap<Chat> classMap)
        {
        }
    }
}
