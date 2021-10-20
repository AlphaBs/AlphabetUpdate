using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using AlphabetUpdateServer.Core;
using Microsoft.Extensions.Configuration;
using AlphabetUpdateServer.Models;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateServer.Services
{
    public class SecureAesStorage : ISecureStorage
    {
        private SecureAesStorage(AesWrapper _aes)
        {
            this.aes = _aes;
            this.path = GeneratePath();
        }

        public SecureAesStorage(IConfiguration conf, ILogger<SecureAesStorage> _logger)
        {
            logger = _logger;
            aes = new AesWrapper(conf["SecureStorageKey"], conf["SecureStorageIV"]);
            path = GeneratePath();
        }

        public static SecureAesStorage FromAes(AesWrapper aes)
        {
            var inst = new SecureAesStorage(aes);
            return inst;
        }

        public static SecureAesStorage FromAes(Aes aes)
        {
            var inst = new SecureAesStorage(new AesWrapper(aes));
            return inst;
        }

        private readonly ILogger<SecureAesStorage>? logger;
        private readonly AesWrapper aes;
        private readonly string path;
        private SecureKeys? obj;

        public SecureKeys GetObject() => obj ?? throw new NullReferenceException();

        public string GeneratePath()
        {
            string newPath;

            if (OperatingSystem.IsWindows())
            {
                newPath = $"{Environment.GetEnvironmentVariable("appdata")}\\.alphabetKey";
            }
            else if (OperatingSystem.IsLinux())
            {
                newPath = Path.GetFullPath("~/.alphabetKey");
            }
            else
            {
                newPath = Path.GetFullPath("./alphabetKey");
            }

            return newPath;
        }

        public async Task<SecureKeys> Load()
        {
            await using var fs = File.OpenRead(path);
            var newObj = await aes.AesDecrypt<SecureKeys>(fs);
            if (newObj == null)
            {
                logger?.LogError("Null SecureStorage");
                throw new NoNullAllowedException();
            }
            logger?.LogInformation("New SecureStorage loaded");
            return obj = newObj;
        }

        public async Task Save(SecureKeys saveObj)
        {
            await using var fs = File.OpenWrite(path);
            await aes.AesEncrypt(saveObj, fs);
        }
    }
}
