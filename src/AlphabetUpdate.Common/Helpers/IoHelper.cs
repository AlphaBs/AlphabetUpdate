using System.IO;
using System.Threading.Tasks;

namespace AlphabetUpdate.Common.Helpers
{
    public static class IoHelper
    {
        public static string NormalizePath(string path, bool fullPath=true)
        {
            if (fullPath)
                path = Path.GetFullPath(path);
            return path
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Trim(Path.DirectorySeparatorChar);
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] directories = Directory.GetDirectories(target_dir);
            foreach (string path in files)
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
            foreach (string target_dir1 in directories)
                DeleteDirectory(target_dir1);
            Directory.Delete(target_dir, true);
        }

        public static async Task CopyFileAsync(string from, string to)
        {
            using var fromStream = File.OpenRead(from);
            using var toStream = File.OpenWrite(to);
            await fromStream.CopyToAsync(toStream);
        }
    }
}