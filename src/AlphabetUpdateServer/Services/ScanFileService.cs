using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AlphabetUpdateServer.Services
{
    public class ScanFileService : IScanFileService
    {
        private readonly ILogger<ScanFileService> logger;
        private readonly UpdateFileOptions options;

        public ScanFileService(
            ILogger<ScanFileService> log,
            IOptions<UpdateFileOptions> opts)
        {
            options = opts.Value;
            logger = log;
        }

        public async Task<UpdateFileCollection> ScanFile()
        {
            logger.LogInformation("Scan files in directory {0}", options.InputDir);
            var list = new List<UpdateFile>();

            var dir = new DirectoryInfo(options.InputDir);
            var files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var underPath = file.FullName
                    .Replace(options.InputDir, "");
                underPath = IoHelper.NormalizePath(underPath, fullPath: false);

                var escapedPath = underPath.Replace('\\', '/');
                var hash = await Task.Run(() => CryptoHelper.HashMd5(file.FullName));
                var f = new UpdateFile
                {
                    Hash = CryptoHelper.ToHexString(hash),
                    Path = escapedPath,
                    Tags = null,
                    Url = null
                };

                list.Add(f);
            }

            logger.LogInformation("{0} files", list.Count);

            return new UpdateFileCollection()
            {
                Files = list.ToArray(),
                HashAlgorithm = "md5",
                LastUpdate = DateTime.Now
            };
        }
    }
}
