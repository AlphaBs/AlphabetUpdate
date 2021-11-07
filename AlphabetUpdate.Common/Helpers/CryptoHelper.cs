using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AlphabetUpdate.Common.Helpers
{
    public class CryptoHelper
    {
        public static string ToHexString(byte[] data)
        {
            return BitConverter.ToString(data)
                .Replace("-", "")
                .ToLowerInvariant();
        }
        
        public static byte[] HashMd5(string path)
        {
            using var stream = File.OpenRead(path);
            return HashMd5(stream);
        }

        public static byte[] HashMd5(Stream stream)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(stream);
        }

        public static byte[] HashSha256(byte[] data)
        {
            using SHA256 hasher = SHA256.Create();
            return hasher.ComputeHash(data);
        }

        public static string HashSha256(string str)
        {
            var data = Encoding.UTF8.GetBytes(str);
            return ToHexString(data);
        }

        public static byte[] GenerateRandomBytes(int length)
        {
            using var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[length];
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}