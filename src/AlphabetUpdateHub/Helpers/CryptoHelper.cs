using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace AlphabetUpdateHub.Helpers
{
    public class CryptoHelper
    {
        public static string HashMd5(string path)
        {
            using var stream = File.OpenRead(path);
            return HashMd5(stream);
        }

        public static string HashMd5(Stream stream)
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
    }
}