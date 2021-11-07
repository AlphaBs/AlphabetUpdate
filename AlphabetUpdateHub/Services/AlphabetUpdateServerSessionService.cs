using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateHub.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlphabetUpdateHub.Services
{
    public class AlphabetUpdateServerSessionService
    {
        private readonly IMongoCollection<UpdateServerToken> _sessions;

        public AlphabetUpdateServerSessionService(IOptions<DatabaseSettings> opt)
        {
            var settings = opt.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _sessions = database.GetCollection<UpdateServerToken>(settings.UpdateServerSessionCollectionName);
        }
        
        public List<UpdateServerToken> Get() =>
            _sessions.Find(x => true).ToList();

        public Task<UpdateServerToken?> GetByServerId(string serverId) =>
            _sessions.Find(x => x.ServerId == serverId).FirstOrDefaultAsync()!;

        public Task Create(UpdateServerToken token) =>
            _sessions.InsertOneAsync(token);

        public Task Update(UpdateServerToken tokenIn) =>
            _sessions.ReplaceOneAsync(c => c.ServerId == tokenIn.ServerId, tokenIn);

        public Task CreateOrUpdate(UpdateServerToken tokenIn) =>
            _sessions.ReplaceOneAsync(c => c.ServerId == tokenIn.ServerId, tokenIn, new ReplaceOptions
            {
                IsUpsert = true
            });

        public Task Delete(AlphabetUpdateServer tokenIn) =>
            _sessions.DeleteManyAsync(c => c.ServerId == tokenIn.ServerId);

        public Task Delete(string serverId) =>
            _sessions.DeleteOneAsync(c => c.ServerId == serverId);
    }
}