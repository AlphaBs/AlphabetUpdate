using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.UpdateServer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlphabetUpdateHub.Services
{
    public class LauncherMetadataCacheService
    {
        private readonly IMongoCollection<LauncherMetadataCache> caches;

        public LauncherMetadataCacheService(IOptions<DatabaseSettings> opt)
        {
            var settings = opt.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            caches = database.GetCollection<LauncherMetadataCache>(settings.LauncherCacheCollectionName);
        }
        
        public Task<LauncherMetadataCache?> GetByServerId(string id) =>
            caches.Find(x => x.ServerId == id).FirstOrDefaultAsync()!;

        public Task Create(LauncherMetadataCache cache) =>
            caches.InsertOneAsync(cache);

        public Task Update(LauncherMetadataCache @in) =>
            caches.ReplaceOneAsync(c => c.ServerId == @in.ServerId, @in);

        public Task CreateOrUpdate(LauncherMetadataCache @in) =>
            caches.ReplaceOneAsync(c => c.ServerId == @in.ServerId, @in, new ReplaceOptions
            {
                IsUpsert = true
            });

        public Task Delete(LauncherMetadataCache @in) =>
            caches.DeleteOneAsync(c => c.ServerId == @in.ServerId);

        public Task Delete(string serverId) =>
            caches.DeleteOneAsync(c => c.ServerId == serverId);

        public async Task<LauncherMetadata?> UpdateLauncherMetadata(AlphabetUpdateServer server)
        {
            var api = new AlphabetUpdateServerApi(server);
            var metadata = await api.GetMetadata();

            if (metadata == null)
                throw new NullReferenceException(nameof(metadata));
                
            var cache = LauncherMetadataCache.Create(server.ServerId, metadata);
            await this.CreateOrUpdate(cache);

            return cache.LauncherMetadata;
        }
    }
}