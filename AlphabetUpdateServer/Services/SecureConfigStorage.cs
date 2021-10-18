using System.Threading.Tasks;
using AlphabetUpdateServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateServer.Services
{
    public class SecureConfigStorage : ISecureStorage
    {
        public SecureConfigStorage(IConfiguration _conf, ILogger<SecureAesStorage> _logger)
        {
            conf = _conf;
            logger = _logger;
            obj = new SecureKeys();
        }

        private readonly IConfiguration conf;
        private readonly ILogger<SecureAesStorage>? logger;
        private SecureKeys obj;

        public SecureKeys GetObject() => obj;
        
        public Task<SecureKeys> Load()
        {
            conf.GetSection("SecureStorage").Bind(obj);
            logger?.LogInformation("New SecureStorage loaded");
            return Task.FromResult(obj);
        }

        public Task Save(SecureKeys saveObj)
        {
            return Task.CompletedTask;
        }
    }
}