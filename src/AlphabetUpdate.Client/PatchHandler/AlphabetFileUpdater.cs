using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using log4net;

namespace AlphabetUpdate.Client.PatchHandler
{    
    public class AlphabetFileUpdater : IPatchHandler
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(AlphabetFileUpdater));
        
        public AlphabetFileUpdater(
            UpdateFileCollection _files,
            AlphabetFileUpdaterOptions _options)
        {
            updateFileCollection = _files;
            options = _options;
        }

        public event FileChangedEventHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;

        private readonly UpdateFileCollection updateFileCollection;
        private readonly AlphabetFileUpdaterOptions options;

        private WebClient initializeWebClient()
        {
            var webClient = new WebClient();
            webClient.DownloadProgressChanged += (sender, args)
                => ProgressChanged?.Invoke(this, args);
            return webClient;
        }
        
        public async Task Patch(PatchContext context)
        {
            if (options.LastUpdate == null)
            {
                if (!string.IsNullOrEmpty(options.LastUpdateFilePath) && 
                    File.Exists(options.LastUpdateFilePath))
                {
                    logger.Info("read LastUpdate from " + options.LastUpdateFilePath);
                    var content = File.ReadAllText(options.LastUpdateFilePath);
                    if (DateTime.TryParse(content, out DateTime result))
                        options.LastUpdate = result;
                }
            }

            if (options.LastUpdate == null)
                options.LastUpdate = DateTime.MinValue;
            
            logger.Info($"options.LastUpdate: {options.LastUpdate}, " +
                        $"updateFileCollection.LastUpdate: {updateFileCollection.LastUpdate}");
            
            await DownloadFiles(context);

            if (options.LastUpdate < updateFileCollection.LastUpdate)
            {
                DeleteInvalidFiles(context);

                if (!string.IsNullOrEmpty(options.LastUpdateFilePath))
                {
                    var content = updateFileCollection.LastUpdate.ToString("o");
                    var lastUpdateFileDir = Path.GetDirectoryName(options.LastUpdateFilePath);
                    if (!string.IsNullOrEmpty(lastUpdateFileDir))
                        Directory.CreateDirectory(lastUpdateFileDir);
                    File.WriteAllText(options.LastUpdateFilePath, content);
                    logger.Info("write LastUpdate to " + options.LastUpdateFilePath);
                }
            }
            else if (options.AlwaysUpdates != null)
            {
                // keep always up-to-date
                foreach (var item in options.AlwaysUpdates)
                {
                    var path = Path.Combine(context.ClientPath, item);
                    DeleteInvalidFiles(context, path);
                }
            }
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
            var webClient = initializeWebClient();
            
            int progressed = 0;
            foreach (var item in updateFileCollection.Files)
            {
                FileChanged?.Invoke(this, new FileChangedEventArg
                {
                    NowFileType = "mod",
                    NowFileName = item.Path,
                    TotalFileCount = updateFileCollection.Files.Length,
                    ProgressedFileCount = progressed
                });
                
                var path = IoHelper.NormalizePath(Path.Combine(context.ClientPath, item.Path));
                var disabledPath = path + "_b";
                
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
                        await CheckAndDownloadFile(webClient, path, item);

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
            }
            
            webClient.Dispose();
        }

        private async Task CheckAndDownloadFile(WebClient webClient, string path, UpdateFile file)
        {
            for (int i = 0; i < options.RetryCount; i++)
            {
                try
                {
                    if (CheckFileValidation(path, file.Hash))
                        return;

                    var url = GetUrl(file);
                    if (string.IsNullOrEmpty(url))
                    {
                        if (options.SkipEmptyUrl)
                            return;
                        else
                            throw new PatchException("No url");
                    }

                    await webClient.DownloadFileTaskAsync(url, path);

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
            DeleteInvalidFiles(context, context.ClientPath);
        }
        
        public void DeleteInvalidFiles(PatchContext context, string targetPath)
        {
            logger.Info("DeleteInvalidFiles: " + targetPath);
            
            if (!Directory.Exists(targetPath))
                return;

            var f = GetTargetFiles(context, targetPath)
                .Except(updateFileCollection.Files.Select(x
                    => IoHelper.NormalizePath(Path.Combine(context.ClientPath, x.Path))));

            f = f.Where(x => !context.IsWhitelistFile(x));

            int count = 0;
            foreach (var path in f)
            {
                File.Delete(path);
                count++;
            }
            
            logger.Info("delete files: " + count);
        }
    }
}