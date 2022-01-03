using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlphabetUpdateServer.Services
{
    public class LauncherService : ILauncherService
    {
        private readonly UpdateFileOptions options;
        private readonly string launcherInfoPath;
        private readonly string filesPath;
        private readonly string launcherCachePath;
        private readonly ILogger<LauncherService> logger;

        public LauncherService(
            ILogger<LauncherService> log,
            IWebHostEnvironment _env, 
            IOptions<UpdateFileOptions> updateFileOptions)
        {
            this.logger = log;
            this.options = updateFileOptions.Value;
            IWebHostEnvironment env = _env;
            
            var webRoot = env.WebRootPath ?? env.ContentRootPath ?? "./";
            launcherInfoPath = Path.Combine(webRoot, options.LauncherInfoPath);
            filesPath = Path.Combine(webRoot, options.FilesCachePath);
            launcherCachePath = Path.Combine(webRoot, "launcher.json");
        }

        public async Task<UpdateFileCollection> GetFiles()
        {
            var json = await GetFilesCache();
            return JsonSerializer.Deserialize<UpdateFileCollection>(json, JsonHelper.JsonOptions)
                   ?? new UpdateFileCollection();
        }
        
        public async Task<string> GetFilesCache()
        {
            if (!File.Exists(filesPath))
                throw new FileNotFoundException("No files cache");
            
            return await File.ReadAllTextAsync(filesPath);
        }

        public async Task<string> UpdateFiles(UpdateFileCollection updateFiles)
        {
            await Util.WriteJson(filesPath, updateFiles);
            logger.LogInformation("FilesCache updated");
            return await Update(null, updateFiles);
        }

        public async Task DeleteFiles()
        {
            if (File.Exists(filesPath))
            {
                await Task.Run(() => File.Delete(filesPath));
                logger.LogInformation("FilesCache deleted: {Path}", filesPath);
            }
        }

        public async Task<LauncherInfo> GetInfo()
        {
            var json = await GetInfoCache();
            return JsonSerializer.Deserialize<LauncherInfo>(json, JsonHelper.JsonOptions)
                ?? new LauncherInfo();
        }
        
        public async Task<string> GetInfoCache()
        {
            if (!File.Exists(launcherInfoPath))
                throw new FileNotFoundException("No info cache");

            return await File.ReadAllTextAsync(launcherInfoPath);
        }

        public async Task<string> UpdateInfo(LauncherInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
                throw new ArgumentException("No Name");
            if (string.IsNullOrEmpty(info.LauncherServer))
                throw new ArgumentException("No LauncherServer");
            if (string.IsNullOrEmpty(info.StartVersion))
                throw new ArgumentException("No StartVersion");
            
            await Util.WriteJson(launcherInfoPath, info);
            logger.LogInformation("InfoCache updated");
            return await Update(info, null);
        }

        public async Task DeleteInfo()
        {
            if (File.Exists(launcherInfoPath))
            {
                await Task.Run(() => File.Delete(launcherInfoPath));
                logger.LogInformation("InfoCache deleted");
            }
        }
        
        public async Task<string> GetCache()
        {
            if (!File.Exists(launcherCachePath))
                throw new FileNotFoundException("No cache");
                
            return await File.ReadAllTextAsync(launcherCachePath);
        }
        
        public async Task<string> Update(LauncherInfo? launcherInfo, UpdateFileCollection? updateFiles)
        {
            if (launcherInfo == null)
            {
                if (File.Exists(launcherInfoPath))
                    launcherInfo = await Util.ReadJson<LauncherInfo>(launcherInfoPath);
                else
                    launcherInfo = new LauncherInfo();
            }

            if (updateFiles == null)
            {
                if (File.Exists(filesPath))
                    updateFiles = await Util.ReadJson<UpdateFileCollection>(filesPath);
                else
                    updateFiles = new UpdateFileCollection();
            }

            var obj = new
            {
                LastUpdate = DateTime.Now,
                Launcher = launcherInfo,
                Files = updateFiles
            };

            var json = await Util.WriteJson(launcherCachePath, obj);
            return json;
        }
    }
}