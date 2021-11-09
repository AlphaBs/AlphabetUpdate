using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdateHub.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlphabetUpdateHub.Services
{
    public class AccountService
    {
        private readonly IMongoCollection<Account> _accounts;

        public AccountService(IOptions<DatabaseSettings> opts)
        {
            var settings = opts.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _accounts = database.GetCollection<Account>(settings.AccountCollectionName);
        }

        public Task<List<Account>> Get() =>
            _accounts.Find(x => true).ToListAsync();

        public Task<Account> GetById(string id) =>
            _accounts.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Task Create(Account account) =>
            _accounts.InsertOneAsync(account);

        public Task Update(Account account) =>
            _accounts.ReplaceOneAsync(c => c.Id == account.Id, account);

        public Task CreateOrUpdate(Account account) =>
            _accounts.ReplaceOneAsync(c => c.Id == account.Id, account, new ReplaceOptions
            {
                IsUpsert = true
            });

        public Task Delete(Account account) =>
            _accounts.DeleteManyAsync(c => c.Id == account.Id);

        public Task Delete(string id) =>
            _accounts.DeleteOneAsync(c => c.Id == id);
    }
}