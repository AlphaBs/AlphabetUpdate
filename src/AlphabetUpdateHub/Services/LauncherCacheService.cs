using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.UpdateServer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlphabetUpdateHub.Services
{
    public class LauncherCacheService
    {
        private readonly IMongoCollection<LauncherMetadataCache> _caches;

        public LauncherCacheService(IOptions<DatabaseSettings> opt)
        {
            var settings = opt.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _caches = database.GetCollection<LauncherMetadataCache>(settings.LauncherCacheCollectionName);
        }

        public List<LauncherMetadataCache> Get() =>
            _caches.Find(x => true).ToList();
        
        public Task<LauncherMetadataCache?> GetByServerId(string id) =>
            _caches.Find(x => x.ServerId == id).FirstOrDefaultAsync()!;

        public Task Create(LauncherMetadataCache cache) =>
            _caches.InsertOneAsync(cache);

        public Task Update(LauncherMetadataCache cacheIn) =>
            _caches.ReplaceOneAsync(c => c.ServerId == cacheIn.ServerId, cacheIn);

        public Task CreateOrUpdate(LauncherMetadataCache cacheIn) =>
            _caches.ReplaceOneAsync(c => c.ServerId == cacheIn.ServerId, cacheIn, new ReplaceOptions
            {
                IsUpsert = true
            });

        public Task Delete(LauncherMetadataCache cacheIn) =>
            _caches.DeleteOneAsync(c => c.ServerId == cacheIn.ServerId);

        public Task Delete(string serverId) =>
            _caches.DeleteOneAsync(c => c.ServerId == serverId);
        
        public async Task<LauncherMetadata?> GetMetadata(string serverId)
        {
            var cache = await this.GetByServerId(serverId);
            if (cache == null || cache.LastMetadataUpdate.AddMinutes(60 * 24) > DateTime.Now)
                return null;

            return cache.LauncherMetadata;
        }

        public async Task<LauncherMetadata?> UpdateMetadata(AlphabetUpdateServer server)
        {
            var api = new AlphabetUpdateServerApi(server);
            var metadata = await api.GetMetadata();

            if (metadata == null)
                throw new NullReferenceException(nameof(metadata));
                
            var cache = new LauncherMetadataCache
            {
                ServerId = server.ServerId,
                LastMetadataUpdate = DateTime.Now,
                LauncherMetadata = metadata
            };

            await this.CreateOrUpdate(cache);

            return cache.LauncherMetadata;
        }
    }
}