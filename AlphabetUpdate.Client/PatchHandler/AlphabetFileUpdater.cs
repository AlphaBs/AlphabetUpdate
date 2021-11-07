using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using CmlLib.Core;
using CmlLib.Core.Downloader;
using log4net;
using log4net.Core;

namespace AlphabetUpdate.Client.Updater
{
    public class AlphabetFileUpdaterOptions
    {
        public string? BaseUrl { get; set; }
        public bool CheckHash { get; set; } = true;
        public int RetryCount { get; set; } = 3;
    }
    
    public class AlphabetFileUpdater : IPatchHandler, IDisposable
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(AlphabetFileUpdater));
        
        public AlphabetFileUpdater(
            UpdateFileCollection _files,
            AlphabetFileUpdaterOptions _options)
        {
            updateFileCollection = _files;
            options = _options;
        }

        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;

        private readonly UpdateFileCollection updateFileCollection;
        private readonly AlphabetFileUpdaterOptions options;

        private WebClient? webClient;

        private void initializeWebClient()
        {
            webClient = new WebClient();
            webClient.DownloadProgressChanged += (sender, args)
                => ProgressChanged?.Invoke(this, args);
        }
        
        public async Task Patch(PatchContext context)
        {
            await DownloadFiles(context);
            DeleteInvalidFiles(context);
            this.Dispose();
        }
        
        public List<string> GetTargetFiles(PatchContext context, string path)
        {
            var list = new List<string>();
            GetTargetFiles(context, path, list);
            return list;
        }
        
        private void GetTargetFiles(PatchContext context, string dir, List<string> results)
        {
            var directoryInfo = new DirectoryInfo(dir);
            var files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                .Select(x => x.FullName);
            
            results.AddRange(files);

            var dirs = directoryInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
            foreach (var item in dirs)
            {
                var realPath = Path.Combine(dir, item.Name);
                if (context.IsWhitelistDir(realPath))
                    continue;
                GetTargetFiles(context, realPath, results);
            }
        }
        
        public async Task DownloadFiles(PatchContext context)
        {
            if (webClient == null)
                initializeWebClient();
            
            int progressed = 0;
            foreach (var item in updateFileCollection.Files)
            {
                var path = Path.Combine(context.MinecraftPath.BasePath, item.Path);
                var disabledPath = context.MinecraftPath + "_b";
                
                var dirPath = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dirPath))
                    Directory.CreateDirectory(dirPath);

                if (context.IsTagContains(item.Tags))
                {
                    if (File.Exists(disabledPath))
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                        File.Move(disabledPath, path);
                    }
                    
                    if (!context.IsWhitelistFile(path))
                        await CheckAndDownloadFile(path, item);

                    if (!context.IsIgnoreTagContains(item.Tags))
                        context.GetTagFilePathList(item.Tags).Add(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        if (File.Exists(disabledPath))
                            File.Delete(disabledPath);
                        
                        if (!context.IsIgnoreTagContains(item.Tags))
                            File.Move(path, disabledPath);
                    }
                }

                progressed++;
                FileChanged?.Invoke(new DownloadFileChangedEventArgs(
                    type: MFile.Others, 
                    source: this, 
                    filename: item.Path, 
                    total: updateFileCollection.Files.Length, 
                    progressed: progressed));
            }
        }

        private async Task CheckAndDownloadFile(string path, UpdateFile file)
        {
            for (int i = 0; i < options.RetryCount; i++)
            {
                try
                {
                    if (CheckFileValidation(path, file.Hash))
                        return;

                    var url = GetUrl(file);
                    await webClient!.DownloadFileTaskAsync(url, path);

                    if (CheckFileValidation(path, file.Hash))
                        return;
                }
                catch (Exception ex)
                {
                    logger.Error($"CheckAndDownloadFile, try {i+1} times");
                    logger.Error(ex);
                }
            }
        }

        private string GetUrl(UpdateFile file)
        {
            if (string.IsNullOrEmpty(file.Url))
            {
                if (string.IsNullOrEmpty(options.BaseUrl))
                    return "";
                
                var url = options.BaseUrl;
                if (!url.EndsWith("/"))
                    url += "/";
                return url + file.Path.Replace('\\', '/');
            }

            return file.Url;
        }

        private bool CheckFileValidation(string path, string compareHash)
        {
            if (!File.Exists(path))
                return false;

            if (!options.CheckHash)
                return true;

            var computedHash = CryptoHelper.ToHexString(CryptoHelper.HashMd5(path));
            return computedHash == compareHash;
        }

        public void DeleteInvalidFiles(PatchContext context)
        {
            var f = GetTargetFiles(context, context.MinecraftPath.BasePath)
                .Except(updateFileCollection.Files.Select(x
                    => IoHelper.NormalizePath(Path.Combine(context.MinecraftPath.BasePath, x.Path))));

            f = f.Where(x => !context.IsWhitelistFile(x));

            foreach (var path in f)
            {
                File.Delete(path);
            }
        }

        public void Dispose()
        {
            webClient?.Dispose();
            webClient = null;
        }
    }
}