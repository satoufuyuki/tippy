using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tippy.Database.Models;

namespace Tippy.Database
{
    public class Manager
    {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<User> _usersCollection;
        private IMongoCollection<Guild> _guildsCollection;

        public Manager(string connectionString)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("tippy");
            _usersCollection = _database.GetCollection<User>("users");
            _guildsCollection = _database.GetCollection<Guild>("guilds");
        }

        public async Task InsertGuild(Guild guild)
        {
            await _guildsCollection.InsertOneAsync(guild);
        }

        public async Task<List<Guild>> GetAllGuilds()
        {
            return await _guildsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<Guild>> GetGuildByField(string fieldName, string fieldValue)
        {
            var filter = Builders<Guild>.Filter.Eq(fieldName, fieldValue);
            var result = await _guildsCollection.Find(filter).ToListAsync();

            return result;
        }

        public async Task<List<Guild>> GetGuilds(int startingFrom, int count)
        {
            var result = await _guildsCollection.Find(new BsonDocument())
            .Skip(startingFrom)
            .Limit(count)
            .ToListAsync();

            return result;
        }

        public async Task<bool> UpdateGuild(string id, string udateFieldName, string updateFieldValue)
        {
            var filter = Builders<Guild>.Filter.Eq("GuildId", id);
            var update = Builders<Guild>.Update.Set(udateFieldName, updateFieldValue);

            var result = await _guildsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount != 0;
        }

        public async Task<bool> DeleteGuildById(string id)
        {
            var filter = Builders<Guild>.Filter.Eq("GuildId", id);
            var result = await _guildsCollection.DeleteOneAsync(filter);
            return result.DeletedCount != 0;
        }

        public async Task<long> DeleteAllGuilds()
        {
            var filter = new BsonDocument();
            var result = await _guildsCollection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task InsertUser(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _usersCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<User>> GetUsersByField(string fieldName, string fieldValue)
        {
            var filter = Builders<User>.Filter.Eq(fieldName, fieldValue);
            var result = await _usersCollection.Find(filter).ToListAsync();

            return result;
        }

        public async Task<List<User>> GetUsers(int startingFrom, int count)
        {
            var result = await _usersCollection.Find(new BsonDocument())
            .Skip(startingFrom)
            .Limit(count)
            .ToListAsync();

            return result;
        }

        public async Task<bool> UpdateUser(string id, string udateFieldName, string updateFieldValue)
        {
            var filter = Builders<User>.Filter.Eq("UserId", id);
            var update = Builders<User>.Update.Set(udateFieldName, updateFieldValue);

            var result = await _usersCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount != 0;
        }

        public async Task<bool> DeleteUserById(string id)
        {
            var filter = Builders<User>.Filter.Eq("UserId", id);
            var result = await _usersCollection.DeleteOneAsync(filter);
            return result.DeletedCount != 0;
        }

        public async Task<long> DeleteAllUsers()
        {
            var filter = new BsonDocument();
            var result = await _usersCollection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }
    }
}
