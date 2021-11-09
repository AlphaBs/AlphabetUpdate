using System;
using AlphabetUpdate.Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlphabetUpdateHub.Models
{
    public class UpdateServerMetadata
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }
        public string? ServerId { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public AlphabetUpdateServer? UpdateServer { get; set; }
    }
}