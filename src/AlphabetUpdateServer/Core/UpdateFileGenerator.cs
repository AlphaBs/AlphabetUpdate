using AlphabetUpdateServer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace AlphabetUpdateServer.Core
{
    public class UpdateFileGenerator
    {
        public string InputDir { get; private set; }
        public string OutputDir { get; private set; }

        public UpdateFileGenerator(string inputDir, string outputDir)
        {
            InputDir = inputDir;
            OutputDir = outputDir;
        }

        private string normalizePath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Trim(Path.DirectorySeparatorChar);
        }

        public UpdateFile[] GetUpdateFiles()
        {
            var list = new List<UpdateFile>();

            var dir = new DirectoryInfo(InputDir);
            var files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var underPath = file.FullName
                    .Replace(InputDir, "");
                underPath = normalizePath(underPath);

                string escapedPath = underPath.Replace('\\', '/');
                var f = new UpdateFile
                {
                    Hash = Util.Md5(file.FullName),
                    Path = escapedPath,
                    Tags = null,
                    Url = null
                };
                
                list.Add(f);
            }
            
            return list.ToArray();
        }
    }
}
