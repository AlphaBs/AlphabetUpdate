using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace AlphabetUpdateServer.Core
{
    public class Util
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static string Md5(string path)
        {
            using var stream = File.OpenRead(path);
            return Md5(stream);
        }

        public static string Md5(Stream stream)
        {
            using var md5 = MD5.Create();
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
            var content = JsonSerializer.Serialize(obj, JsonOptions);
            await File.WriteAllTextAsync(path, content);
            return content;
        }

        public static FileStream CreateAsyncReadStream(string sourceFile)
        {
            return new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        }
        
        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            await using var stream = CreateAsyncReadStream(sourceFile);
            await CopyFileAsync(stream, destinationFile);
        }

        public static async Task CopyFileAsync(Stream sourceStream, string destinationFile)
        {
            await using var destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            await sourceStream.CopyToAsync(destinationStream);
        }
        
        public static void DeleteEmptyDirs(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentException(
                    "Starting directory is a null reference or an empty string", 
                    "dir");
            
            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }
}
