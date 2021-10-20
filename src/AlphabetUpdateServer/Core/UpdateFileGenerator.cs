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
        public bool ExplictUrl { get; set; }
        public string DefaultTagName { get; set; } = "common";
        public string InputDir { get; private set; }
        public string OutputDir { get; private set; }
        public string BaseUrl { get; private set; }

        public UpdateFileGenerator(string inputDir, string outputDir, string baseUrl)
        {
            InputDir = inputDir;
            OutputDir = outputDir;
            BaseUrl = baseUrl;
        }

        private string normalizePath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Trim(Path.DirectorySeparatorChar);
        }

        public async Task<UpdateFile[]> GetTagUpdateFiles()
        {
            var list = new List<UpdateFile>();
            foreach (var tag in new DirectoryInfo(InputDir).GetDirectories())
            {
                var files = await GetUpdateFiles(tag.Name, new [] { tag.Name });
                list.AddRange(files);
            }

            return list.ToArray();
        }

        private async Task<UpdateFile[]> GetUpdateFiles(string midPath, string[] tags)
        {
            var list = new List<UpdateFile>();

            var path = Path.Combine(InputDir, midPath);
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            
            var outFilesArr = Directory.GetFiles(OutputDir, "*.*", SearchOption.AllDirectories)
                .Select(x => normalizePath(x).ToLowerInvariant());
            var outFiles = new HashSet<string>(outFilesArr);

            string? tagStr;
            if (tags.Length == 1 && tags[0] == DefaultTagName)
                tagStr = null;
            else
                tagStr = string.Join(',', tags);

            foreach (var file in files)
            {
                var underPath = file.FullName
                    .Replace(path, "");
                underPath = normalizePath(underPath);

                var outFilePath = normalizePath(Path.Combine(OutputDir, underPath));
                var outFileDirPath = Path.GetDirectoryName(outFilePath);
                if (!string.IsNullOrEmpty(outFileDirPath) && !Directory.Exists(outFileDirPath))
                    Directory.CreateDirectory(outFileDirPath);

                var outputStream = Util.CreateAsyncReadStream(file.FullName);
                var copyTask = Util.CopyFileAsync(outputStream, outFilePath);

                string escapedPath = underPath.Replace('\\', '/');
                string? url = null;
                if (ExplictUrl)
                    url = $"{BaseUrl}/{escapedPath}";
                
                var f = new UpdateFile
                {
                    Hash = Util.Md5(outputStream),
                    Path = escapedPath,
                    Tags = tagStr,
                    Url = url
                };
                
                list.Add(f);
                outFiles.Remove(outFilePath.ToLowerInvariant());

                await copyTask;
                await outputStream.DisposeAsync();
            }

            foreach (var remainOutFile in outFiles)
            {
                File.Delete(remainOutFile);
            }
            
            return list.ToArray();
        }
    }
}
