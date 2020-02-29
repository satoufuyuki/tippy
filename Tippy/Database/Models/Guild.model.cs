using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tippy.Database.Models
{
    public class Guild
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string GuildId { get; set; }

        [BsonElement]
        public string Prefix { get; set; }

        public string WelcomeChannel { get; set; }
        
        [BsonElement]
        public string LeaveChannel { get; set; }

        [BsonElement]
        public BsonBoolean EnabledWelcome { get; set; }

        [BsonElement]
        public BsonBoolean EnabledLeave { get; set; }
    }
}
