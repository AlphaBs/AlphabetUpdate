using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlphabetUpdateHub.Models
{
    public class AlphabetUpdateServer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string? ServerName { get; set; }
        public string? ServerId { get; set; }
        public string? Host { get; set; }
        public string? Password { get; set; }
        public string? AesKey { get; set; }
        public string? AesIv { get; set; }
    }
}