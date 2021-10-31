using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.UpdateServer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlphabetUpdateHub.Services
{
    public class UpdateServerMetadataService
    {
        private readonly IMongoCollection<UpdateServerMetadata> servers;

        public UpdateServerMetadataService(IOptions<DatabaseSettings> opt)
        {
            var settings = opt.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            servers = database.GetCollection<UpdateServerMetadata>(settings.LauncherCacheCollectionName);
        }

        public List<UpdateServerMetadata> Get() =>
            servers.Find(x => true).ToList();
        
        public Task<UpdateServerMetadata?> GetByServerId(string id) =>
            servers.Find(x => x.ServerId == id).FirstOrDefaultAsync()!;

        public Task Create(UpdateServerMetadata cache) =>
            servers.InsertOneAsync(cache);

        public Task Update(UpdateServerMetadata @in) =>
            servers.ReplaceOneAsync(c => c.ServerId == @in.ServerId, @in);

        public Task CreateOrUpdate(UpdateServerMetadata @in) =>
            servers.ReplaceOneAsync(c => c.ServerId == @in.ServerId, @in, new ReplaceOptions
            {
                IsUpsert = true
            });

        public Task Delete(UpdateServerMetadata @in) =>
            servers.DeleteOneAsync(c => c.ServerId == @in.ServerId);

        public Task Delete(string serverId) =>
            servers.DeleteOneAsync(c => c.ServerId == serverId);

        private static readonly ProjectionDefinition<UpdateServerMetadata, UpdateServerMetadata> projectionDefinition =
            Builders<UpdateServerMetadata>.Projection
                .Include(x => x.ServerId)
                .Include(x => x.DisplayName)
                .Include(x => x.Description);
        
        public async Task<List<UpdateServerMetadata>> GetServers(string[] serverIds)
        {
            return await servers
                .Find(x => serverIds.Contains(x.ServerId))
                .Project(projectionDefinition)
                .ToListAsync();
        }
    }
}