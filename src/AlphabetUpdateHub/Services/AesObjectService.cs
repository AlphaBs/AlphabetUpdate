using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;

namespace AlphabetUpdateHub.Services
{
    public class AesObjectService : IAesObjectService
    {
        const int MaxSize = 1024 * 1024;

        public AesObjectService()
            : this(Aes.Create())
        {
            
        }

        public AesObjectService(Aes aes)
        {
            this.aes = aes;
        }

        public AesObjectService(string key, string iv)
        {
            aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);
            aes.Padding = PaddingMode.PKCS7;
        }

        private readonly Aes aes;

        public async Task AesEncrypt(object? obj, Stream writeTo)
        {
            var json = JsonSerializer.Serialize(obj);
            var data = Encoding.UTF8.GetBytes(json);
            var dataHash = CryptoHelper.HashSha256(data);
            
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var csEnc = new CryptoStream(writeTo, encryptor, CryptoStreamMode.Write, leaveOpen: true))
            {
                await writeTo.WriteAsync(dataHash, 0, 32);
                await writeTo.WriteAsync(BitConverter.GetBytes(data.Length), 0, 4);
                await csEnc.WriteAsync(data, 0, data.Length);
            }
        }

        public async Task<T?> AesDecrypt<T>(Stream stream)
        {
            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            await using var csDes = new CryptoStream(stream, decrypt, CryptoStreamMode.Read);
            
            var hashBuffer = new byte[32];
            await stream.ReadAsync(hashBuffer, 0, 32);

            var lengthBuffer = new byte[4];
            await stream.ReadAsync(lengthBuffer, 0, 4);
            var length = BitConverter.ToInt32(lengthBuffer);

            if (length > MaxSize)
                throw new SecurityException("too big");

            var dataBuffer = new byte[length];
            await csDes.ReadAsync(dataBuffer, 0, length);

            var plainHash = CryptoHelper.HashSha256(dataBuffer);

            if (!hashBuffer.SequenceEqual(plainHash))
                throw new SecurityException();

            var plainText = Encoding.UTF8.GetString(dataBuffer);
            var obj = JsonSerializer.Deserialize<T>(plainText, JsonHelper.JsonOptions);

            return obj;
        }
    }
}