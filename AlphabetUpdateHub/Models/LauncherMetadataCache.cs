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
        public string Id { get; set; }
        public string ServerId { get; set; }
        public DateTime LastUpdate { get; set; }
        public LauncherMetadata Launcher { get; set; }

        public static LauncherMetadataCache Create(string serverId, LauncherMetadata metadata)
        {
            return new LauncherMetadataCache
            {
                ServerId = serverId,
                LastUpdate = DateTime.Now,
                Launcher = metadata
            };
        }
    }
}