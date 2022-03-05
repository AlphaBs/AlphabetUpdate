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

namespace AlphabetUpdateServer.Services.Updater
{
    public class OutputDirectoryUpdater : IUpdateService
    {
        private readonly UpdateFileOptions options;
        private readonly ILogger<OutputDirectoryUpdater> logger;
        
        public OutputDirectoryUpdater(
            IOptions<UpdateFileOptions> opts,
            ILogger<OutputDirectoryUpdater> log)
        {
            options = opts.Value;
            logger = log;
        }
        
        public async Task<UpdateFileCollection> Update(UpdateFileCollection updateFiles)
        {
            if (updateFiles.Files == null)
                return updateFiles;

            logger.LogInformation("Output directory: {0}", options.OutputDir);

            if (!Directory.Exists(options.OutputDir))
                Directory.CreateDirectory(options.OutputDir);
            var outFilesArr = Directory.GetFiles(options.OutputDir, "*.*", SearchOption.AllDirectories);
            var outFilesSet = new HashSet<string>();

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
                outFilesSet.Remove(outFilePath);
            }

            foreach (var remainFile in outFilesSet)
            {
                await Task.Run(() => File.Delete(remainFile));
                logger.LogInformation("Delete remain file: {Name}", remainFile);
            }

            IoHelper.DeleteEmptyDirectories(options.OutputDir);
            updateFiles.LastUpdate = DateTime.Now;

            return updateFiles;
        }
    }
}