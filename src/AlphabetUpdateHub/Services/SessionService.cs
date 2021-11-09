using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdateHub.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlphabetUpdateHub.Services
{
    public class SessionService
    {
        private readonly IMongoCollection<Session> _sessions;

        public SessionService(IOptions<DatabaseSettings> opts)
        {
            var settings = opts.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _sessions = database.GetCollection<Session>(settings.SessionCollectionName);
        }

        public Task<List<Session>> Get() =>
            _sessions.Find(x => true).ToListAsync();

        public Task<Session> GetByToken(string token) =>
            _sessions.Find(x => x.Token == token).FirstOrDefaultAsync();

        public Task<Session> GetById(string id) =>
            _sessions.Find(x => x.Id == id).FirstOrDefaultAsync();
        
        public Task Create(Session session) =>
            _sessions.InsertOneAsync(session);

        public Task Update(Session session) =>
            _sessions.ReplaceOneAsync(s => s.Id == session.Id, session);

        public Task CreateOrUpdate(Session session) =>
            _sessions.ReplaceOneAsync(s => s.Id == session.Id, session, new ReplaceOptions
            {
                IsUpsert = true
            });

        public Task Delete(Session session) =>
            _sessions.DeleteManyAsync(s => s.Id == session.Id);

        public Task Delete(string id) =>
            _sessions.DeleteOneAsync(s => s.Id == id);
    }
}