using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlphabetUpdateServer.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly UpdateFileOptions options;
        private readonly ILogger<UpdateService> logger;
        
        public UpdateService(
            IOptions<UpdateFileOptions> opts,
            ILogger<UpdateService> log)
        {
            options = opts.Value;
            logger = log;
        }

        public async Task<UpdateFileCollection> ScanFiles()
        {
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

            return new UpdateFileCollection()
            {
                Files = list.ToArray(),
                HashAlgorithm = "md5",
                LastUpdate = DateTime.Now
            };
        }
        
        public async Task<UpdateFileCollection> UpdateFiles(UpdateFileCollection updateFiles)
        {
            if (updateFiles.Files == null)
                return updateFiles;
            
            if (!Directory.Exists(options.OutputDir))
                Directory.CreateDirectory(options.OutputDir);
            var outFilesArr = Directory.GetFiles(options.OutputDir, "*.*", SearchOption.AllDirectories);
            var outFilesSet = new HashSet<string>(outFilesArr);

            foreach (var inFile in updateFiles.Files)
            {
                if (string.IsNullOrEmpty(inFile.Path))
                    throw new FileNotFoundException($"'{inFile.Url}' does not have file path");

                var path = IoHelper.NormalizePath(inFile.Path, fullPath: false);
                var realPath = Path.Combine(options.InputDir, path);

                if (!File.Exists(realPath))
                    throw new FileNotFoundException(inFile.Path);

                var outFilePath = Path.Combine(options.OutputDir, path);
                var outFileDir = Path.GetDirectoryName(outFilePath);
                if (!string.IsNullOrEmpty(outFileDir) && !Directory.Exists(outFileDir))
                    Directory.CreateDirectory(outFileDir);

                await IoHelper.CopyFileAsync(realPath, outFilePath);
                logger.LogInformation("Copy update file: {Input} -> {Output}", realPath, outFilePath);
                outFilesSet.Remove(outFilePath.ToLowerInvariant());
            }

            foreach (var remainFile in outFilesSet)
            {
                await Task.Run(() => File.Delete(remainFile));
                logger.LogInformation("Delete remain file: {Name}", remainFile);
            }
            
            Util.DeleteEmptyDirs(options.OutputDir);
            updateFiles.LastUpdate = DateTime.Now;

            return updateFiles;
        }
    }
}