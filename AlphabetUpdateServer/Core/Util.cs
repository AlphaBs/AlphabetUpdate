using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text.Json;

namespace AlphabetUpdateServer.Core
{
    public class Util
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static string Md5(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);

            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public static byte[] HashSha256(byte[] data)
        {
            using SHA256 hasher = SHA256.Create();
            return hasher.ComputeHash(data);
        }

        public static async Task<T?> ReadJson<T>(string path)
        {
            var content = await File.ReadAllTextAsync(path);
            var obj = JsonSerializer.Deserialize<T>(content, JsonOptions);
            return obj;
        }

        public static async Task<string> WriteJson(string path, object? obj)
        {
            var content = JsonSerializer.Serialize(obj);
            await File.WriteAllTextAsync(path, content);
            return content;
        }
    }
}
