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
        public TimeSpan LastRep { get; set; }

        [BsonElement]
        public TimeSpan LastWork { get; set; }
    }
}
