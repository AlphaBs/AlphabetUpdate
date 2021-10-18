using AlphabetUpdateServer.Models;
using System.Collections.Generic;
using System.IO;

namespace AlphabetUpdateServer.Core
{
    public class UpdateFileGenerator
    {
        public string BasePath { get; private set; }
        public string UpdateFilePath { get; private set; }
        public string BaseUrl { get; private set; }

        public UpdateFileGenerator(string basePath, string baseUrl, string path)
        {
            this.BaseUrl = baseUrl;
            this.BasePath = basePath;
            this.UpdateFilePath = path;
        }

        public UpdateFile[] GetTagUpdateFiles()
        {
            var list = new List<UpdateFile>();
            var filesPath = Path.Combine(this.BasePath, this.UpdateFilePath);

            foreach (var tag in new DirectoryInfo(filesPath).GetDirectories())
            {
                var midPath = $"{this.UpdateFilePath}/{tag.Name}";
                var files = GetUpdateFiles(midPath, new [] { tag.Name });
                list.AddRange(files);
            }

            return list.ToArray();
        }

        private UpdateFile[] GetUpdateFiles(string midPath, string[] tags)
        {
            var list = new List<UpdateFile>();

            var path = Path.Combine(BasePath, midPath).Replace('/','\\');
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var filePath = file.FullName
                    .Replace(path, "")
                    .Replace('\\', '/')
                    .Trim('/');

                var f = new UpdateFile
                {
                    Hash = Util.Md5(file.FullName),
                    Path = filePath,
                    Url = $"{BaseUrl}/{midPath}/{filePath.Replace('\\', '/')}",
                    Tags = string.Join(',', tags)
                };
                list.Add(f);
            }

            return list.ToArray();
        }
    }
}
