using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AlphabetUpdate.Client.Patch.Core;
using AlphabetUpdate.Client.Patch.Core.Handlers;
using AlphabetUpdate.Client.Patch.Services;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdate.Client.Patch.Handlers
{    
    public class AlphabetFileUpdater : PatchHandlerBase<AlphabetFileUpdateSetting>
    {
        private readonly ILogger<AlphabetFileUpdater> _logger;
        private readonly IPatchProgressService? _patchProgressContext;
        private readonly IWhitelistFileService? _whitelistService;
        private readonly IFileTagService? _tagService;
        private readonly IFileEnabler? _fileEnabler;

        private readonly HashSet<string> tags = new HashSet<string>();

        public AlphabetFileUpdater(
            ILogger<AlphabetFileUpdater> logger,
            IPatchProgressService? progressContext,
            IWhitelistFileService? whitelistService,
            IFileTagService? tagService, 
            IFileEnabler? fileEnabler)
        {
            this._logger = logger;
            this._patchProgressContext = progressContext;
            this._whitelistService = whitelistService;
            this._tagService = tagService;
            this._fileEnabler = fileEnabler;
        }

        // HttpClient 로 바꿔야함
        private WebClient initializeWebClient()
        {
            var webClient = new WebClient();
            webClient.DownloadProgressChanged += (sender, args)
                => _patchProgressContext?.OnProgressChanged(this, args);
            return webClient;
        }
        
        public override async Task Patch(CancellationToken? cancellationToken)
        {
            if (Setting?.UpdateFiles == null)
                return;

            if (Setting.LastUpdate == null)
            {
                if (!string.IsNullOrEmpty(Setting.LastUpdateFilePath) && 
                    File.Exists(Setting.LastUpdateFilePath))
                {
                    _logger.LogInformation("read LastUpdate from {Path}", Setting.LastUpdateFilePath);
                    var content = await File.ReadAllTextAsync(Setting.LastUpdateFilePath);
                    if (DateTime.TryParse(content, out DateTime result))
                        Setting.LastUpdate = result;
                }

                if (Setting.LastUpdate == null)
                    Setting.LastUpdate = DateTime.MinValue;
            }

            _logger.LogInformation(
                "options.LastUpdate: {LastUpdate1}, updateFileCollection.LastUpdate: {LastUpdate2}",
                Setting.LastUpdate, Setting.UpdateFiles?.LastUpdate);

            tags.Clear();
            if (Setting.Tags != null)
            {
                foreach (var tagname in Setting.Tags)
                {
                    tags.Add(tagname);
                }
            }

            PatchContext.Items["new_version"] = Setting.LastUpdate != Setting.UpdateFiles?.LastUpdate;
            await DownloadFiles(cancellationToken);
        }

        public override async Task PostPatch(CancellationToken? cancellationToken)
        {
            if (Setting?.UpdateFiles != null && !string.IsNullOrEmpty(Setting.LastUpdateFilePath))
            {
                var content = Setting.UpdateFiles.LastUpdate.ToString("o");
                var lastUpdateFileDir = Path.GetDirectoryName(Setting.LastUpdateFilePath);
                if (!string.IsNullOrEmpty(lastUpdateFileDir))
                    Directory.CreateDirectory(lastUpdateFileDir);

                await File.WriteAllTextAsync(Setting.LastUpdateFilePath, content);
                _logger.LogInformation("write LastUpdate to {Path}", Setting.LastUpdateFilePath);
            }
        }
        
        public async Task DownloadFiles(CancellationToken? cancellationToken)
        {
            if (Setting?.UpdateFiles?.Files == null)
                return;

            var updateFileCollection = Setting.UpdateFiles;
            var webClient = initializeWebClient();
            
            int progressed = 0;
            foreach (var item in updateFileCollection.Files)
            {
                _patchProgressContext?.OnFileChanged(this, new FileChangedEventArg
                {
                    NowFileType = "mod",
                    NowFileName = item.Path,
                    TotalFileCount = updateFileCollection.Files.Length,
                    ProgressedFileCount = progressed
                });
                
                var path = IoHelper.NormalizePath(Path.Combine(BasePath, item.Path));
                
                var dirPath = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dirPath))
                    Directory.CreateDirectory(dirPath);

                string[]? tags = null;
                if (!string.IsNullOrWhiteSpace(item.Tags))
                    tags = item.Tags.Split(',');

                var tagOption = GetTagOption(tags);
                if (tagOption == FileTagOptions.Enable)
                {
                    if (_fileEnabler != null && await _fileEnabler.CanEnable(path))
                        await _fileEnabler.EnableFile(path);

                    var isWhitelist = _whitelistService?.CheckWhitelistFile(path) ?? false;
                    var result = await CheckAndDownloadFile(webClient, path, item, checkHash: !isWhitelist);

                    if (Setting.SaveUpdateResultTag)
                        _tagService?.Tags.AddFile(path, "patch");
                }
                else if (tagOption == FileTagOptions.Disable)
                {
                    if (_fileEnabler != null && await _fileEnabler.CanDisable(path))
                        await _fileEnabler.DisableFile(path);
                }

                if (Setting.SaveFileTags && item.Tags != null)
                    _tagService?.Tags.AddFile(path, item.Tags);
                progressed++;
            }
            
            webClient.Dispose();
        }

        private async Task<bool> CheckAndDownloadFile(WebClient webClient, string path, UpdateFile file, bool checkHash)
        {
            int retryCount = 3;
            if (Setting != null)
                retryCount = Setting.RetryCount;

            Exception? lastEx = null;
            bool lastFileCheckResult = false;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    if (CheckFileValidation(path, file.Hash, checkHash: checkHash))
                        return true;

                    var url = GetUrl(file);
                    if (string.IsNullOrEmpty(url))
                    {
                        if (Setting?.SkipEmptyUrl ?? false)
                            return false;
                        throw new PatchException("No url");
                    }

                    await webClient.DownloadFileTaskAsync(url, path);

                    if (lastFileCheckResult = CheckFileValidation(path, file.Hash, checkHash: checkHash))
                        return true;
                }
                catch (PatchException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError("CheckAndDownloadFile, try {RetryCount} times", i+1);
                    _logger.LogError("Exception: {Exception}", ex);
                    lastEx = ex;
                }
            }

            if (lastEx != null)
                throw new PatchException("download fail: " + path, lastEx);
            else if (!lastFileCheckResult)
                throw new PatchException("invalid file hash: " + path);
            else
                throw new PatchException("download fail: " + path);
        }

        private string? GetUrl(UpdateFile file)
        {
            if (string.IsNullOrEmpty(file.Url))
            {
                if (string.IsNullOrEmpty(Setting?.BaseUrl))
                    return "";
                
                var url = Setting.BaseUrl;
                if (!string.IsNullOrEmpty(url) && !url.EndsWith("/"))
                    url += "/";
                return url + file.Path?.Replace('\\', '/');
            }

            return file.Url;
        }

        private bool CheckFileValidation(string path, string? compareHash, bool checkHash)
        {
            if (!File.Exists(path))
                return false;

            if (!checkHash)
                return true;

            if (Setting != null && !Setting.CheckHash)
                return true;

            var computedHash = CryptoHelper.ToHexString(CryptoHelper.HashMd5(path));
            return computedHash == compareHash;
        }

        private FileTagOptions GetTagOption(string[]? tags)
        {
            if (tags == null || tags.Length == 0)
                return FileTagOptions.Enable;

            if (Setting?.TagOptions == null)
                return FileTagOptions.Disable;

            foreach (var tagname in tags)
            {
                if (Setting.TagOptions.TryGetValue(tagname, out var value))
                    return value;
            }

            return Setting.DefaultFileTagOption;
        }
    }
}