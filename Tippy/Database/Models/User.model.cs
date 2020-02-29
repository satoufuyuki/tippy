using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tippy.Database
{
    public class User {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public ulong UserId { get; set; }

        [BsonElement]
        public int Money { get; set; }

        [BsonElement]
        public int Xp { get; set; }

        [BsonElement]
        public int Level { get; set; }
    }
}
