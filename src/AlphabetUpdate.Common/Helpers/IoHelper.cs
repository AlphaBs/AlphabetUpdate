using System.IO;

namespace AlphabetUpdate.Common.Helpers
{
    public class IoHelper
    {
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}