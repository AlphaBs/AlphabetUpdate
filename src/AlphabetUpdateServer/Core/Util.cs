using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlphabetUpdate.Common.Helpers;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace AlphabetUpdateServer.Core
{
    public class Util
    {
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
        
        public static async Task<T?> ReadJson<T>(string path)
        {
            var content = await File.ReadAllTextAsync(path);
            var obj = JsonSerializer.Deserialize<T>(content, JsonHelper.JsonOptions);
            return obj;
        }

        public static async Task<string> WriteJson(string path, object? obj)
        {
            var content = JsonSerializer.Serialize(obj, JsonHelper.JsonOptions);
            await File.WriteAllTextAsync(path, content);
            return content;
        }
    }
}
