using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Tippy.Database
{
    public class User {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public ulong UserId { get; set; }

        [BsonElement]
        public int Rep { get; set; }

        [BsonElement]
        public int Money { get; set; }

        [BsonElement]
        public int Xp { get; set; }

        [BsonElement]
        public int Level { get; set; }

        [BsonElement]
        public TimeSpan LastDaily { get; set; }

        [BsonElement]
        public TimeSpan NextDaily { get; set; }

        [BsonElement]
        public TimeSpan LastRep { get; set; }

        [BsonElement]
        public TimeSpan NextRep { get; set; }

        [BsonElement]
        public TimeSpan LastWork { get; set; }

        [BsonElement]
        public TimeSpan NextWork { get; set; }

        [BsonElement]
        public Boolean IsAfk { get; set; }

        [BsonElement]
        public string AfkReason { get; set; }

        [BsonElement]
        public string AfkAttachment { get; set; }

        [BsonElement]
        public string AfkTime { get; set; }
    }
}
