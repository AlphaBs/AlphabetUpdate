using System;
using AlphabetUpdate.Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlphabetUpdateHub.Models
{
    public class LauncherMetadataCache
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }
        public string? ServerId { get; set; }
        public LauncherMetadata? LauncherMetadata { get; set; }
        public DateTime LastMetadataUpdate { get; set; } = DateTime.MinValue;

        public static LauncherMetadataCache Create(string serverId, LauncherMetadata? metadata)
        {
            return new LauncherMetadataCache
            {
                ServerId = serverId,
                LauncherMetadata = metadata,
                LastMetadataUpdate = DateTime.Now
            };
        }
    }
}