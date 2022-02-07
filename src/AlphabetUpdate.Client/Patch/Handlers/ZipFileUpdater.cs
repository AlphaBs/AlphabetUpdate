using AlphabetUpdate.Client.Patch.Core.Handlers;
using AlphabetUpdate.Client.Patch.Services;
using AlphabetUpdate.Common.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Handlers
{
    public class ZipFileUpdater : PatchHandlerBase<ZipFileUpdateSetting>
    {
        private readonly ILogger<ZipFileUpdater> _logger;
        private readonly IWhitelistFileService _whitelistService;
        private readonly IPatchProgressService _progressService;

        public ZipFileUpdater(
            ILogger<ZipFileUpdater> logger,
            IWhitelistFileService whitelistService, 
            IPatchProgressService progressService)
        {
            _logger = logger;
            _whitelistService = whitelistService;
            _progressService = progressService;
        }

        private bool checkLatestVersion()
        {
            if (Setting == null)
                return true;

            if (Setting.LastUpdate == null)
            {
                if (!string.IsNullOrEmpty(Setting?.LastUpdateFilePath) && 
                    File.Exists(Setting.LastUpdateFilePath))
                {
                    _logger.LogInformation("read LastUpdate from " + Setting.LastUpdateFilePath);
                    var lastUpdateFileContent = File.ReadAllText(Setting.LastUpdateFilePath);
                    if (DateTime.TryParse(lastUpdateFileContent, out DateTime result))
                        Setting.LastUpdate = result;
                }
            }

            if (Setting.LastUpdate == null)
                Setting.LastUpdate = DateTime.MinValue;

            _logger.LogInformation($"Setting.LastUpdate: {Setting.LastUpdate}, " +
                        $"Setting.LatestVersion: {Setting.LatestVersion}");

            return Setting.LastUpdate >= Setting.LatestVersion;
        }

        public override async Task Patch(CancellationToken? cancellationToken)
        {
            _logger.LogInformation("Start ZipFileUpdater");

            if (Setting?.ZipStream == null)
            {
                _logger.LogInformation("ZipStream was null. Skip update");
                return;
            }

            if (checkLatestVersion())
                return;

            var patchDir = new DirectoryInfo(PatchContext.BasePath);
            foreach (var dir in patchDir.GetDirectories())
            {
                try
                {
                    if (_whitelistService.CheckWhitelistDir(dir.FullName))
                        continue;

                    _logger.LogInformation("Delete directory: " + dir.FullName);
                    IoHelper.DeleteDirectory(dir.FullName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("delete exception", ex);
                }
            }

            patchDir.Create();
            await unzip(Setting.ZipStream, patchDir.FullName);

            if (!string.IsNullOrEmpty(Setting.LastUpdateFilePath))
            {
                var content = Setting.LatestVersion.ToString("o");
                var lastUpdateFileDir = Path.GetDirectoryName(Setting.LastUpdateFilePath);
                if (!string.IsNullOrEmpty(lastUpdateFileDir))
                    Directory.CreateDirectory(lastUpdateFileDir);
                File.WriteAllText(Setting.LastUpdateFilePath, content);
                _logger.LogInformation("write LastUpdate to " + Setting.LastUpdateFilePath);
            }
        }

        private async Task unzip(Stream inStream, string path)
        {
            using var s = new ZipInputStream(inStream);
            long length = inStream.Length;

            ZipEntry e;
            while ((e = s.GetNextEntry()) != null)
            {
                var zFile = Path.Combine(path, e.Name);
                var fileName = Path.GetFileName(zFile);

                if (string.IsNullOrEmpty(fileName))
                    continue;

                var dirName = Path.GetDirectoryName(zFile);
                if (!string.IsNullOrEmpty(dirName))
                    Directory.CreateDirectory(dirName);

                using var zFileStream = File.OpenWrite(zFile);
                await s.CopyToAsync(zFileStream);

                ev(s.Position, length);
            }
        }

        private int prevPerc;
        private void ev(long progress, long total)
        {
            int percent = (int)((double)(progress/1024) / (double)(total/1024) * 100);
            if (prevPerc == percent)
                return;

            prevPerc = percent;
            _progressService.OnProgressChanged(this, new ProgressChangedEventArgs(percent, null));
        }
    }
}