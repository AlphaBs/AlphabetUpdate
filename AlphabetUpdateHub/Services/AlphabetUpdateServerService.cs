using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdateHub.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlphabetUpdateHub.Services
{
    public class AlphabetUpdateServerService
    {
        private readonly IMongoCollection<AlphabetUpdateServer> _servers;

        public AlphabetUpdateServerService(IOptions<DatabaseSettings> opt)
        {
            var settings = opt.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _servers = database.GetCollection<AlphabetUpdateServer>(settings.UpdateServersCollectionName);
        }

        public List<AlphabetUpdateServer> Get() =>
            _servers.Find(x => true).ToList();

        public Task<AlphabetUpdateServer?> Get(string id) =>
            _servers.Find(x => x.Id == id).FirstOrDefaultAsync()!;
        
        public Task<AlphabetUpdateServer?> GetByServerId(string id) =>
            _servers.Find(x => x.ServerId == id).FirstOrDefaultAsync()!;

        public Task Create(AlphabetUpdateServer server) =>
            _servers.InsertOneAsync(server);

        public Task Update(AlphabetUpdateServer serverIn) =>
            _servers.ReplaceOneAsync(c => c.ServerId == serverIn.ServerId, serverIn);

        public Task CreateOrUpdate(AlphabetUpdateServer serverIn) =>
            _servers.ReplaceOneAsync(c => c.ServerId == serverIn.ServerId, serverIn, new ReplaceOptions
            {
                IsUpsert = true
            });

        public Task Delete(AlphabetUpdateServer serverIn) =>
            _servers.DeleteManyAsync(c => c.ServerId == serverIn.ServerId);

        public Task Delete(string serverId) =>
            _servers.DeleteOneAsync(c => c.ServerId == serverId);
    }
}