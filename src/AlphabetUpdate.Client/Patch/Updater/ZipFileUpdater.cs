using AlphabetUpdate.Client.Patch.Handler;
using AlphabetUpdate.Common.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Updater
{
    public class ZipFileUpdater : PatchHandlerBase<ZipFileUpdateSetting>
    {
        private bool checkLatestVersion()
        {
            if (options.LastUpdate == null)
            {
                if (!string.IsNullOrEmpty(options.LastUpdateFilePath) && 
                    File.Exists(options.LastUpdateFilePath))
                {
                    logger.Info("read LastUpdate from " + options.LastUpdateFilePath);
                    var lastUpdateFileContent = File.ReadAllText(options.LastUpdateFilePath);
                    if (DateTime.TryParse(lastUpdateFileContent, out DateTime result))
                        options.LastUpdate = result;
                }
            }

            if (options.LastUpdate == null)
                options.LastUpdate = DateTime.MinValue;

            logger.Info($"options.LastUpdate: {options.LastUpdate}, " +
                        $"options.LatestVersion: {options.LatestVersion}");

            return options.LastUpdate >= options.LatestVersion;
        }

        public override async Task Patch(CancellationToken? cancellationToken)
        {
            logger.Info("Start ZipFileUpdater");

            if (options.ZipStream == null)
            {
                logger.Info("ZipStream was null. Skip update");
                return;
            }

            if (checkLatestVersion())
                return;

            var patchDir = new DirectoryInfo(context.ClientPath);
            foreach (var dir in patchDir.GetDirectories())
            {
                try
                {
                    if (context.IsWhitelistDir(dir.FullName))
                        continue;

                    logger.Info("Delete directory: " + dir.FullName);
                    IoHelper.DeleteDirectory(dir.FullName);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            patchDir.Create();
            await unzip(options.ZipStream, patchDir.FullName);

            if (!string.IsNullOrEmpty(options.LastUpdateFilePath))
            {
                var content = options.LatestVersion.ToString("o");
                var lastUpdateFileDir = Path.GetDirectoryName(options.LastUpdateFilePath);
                if (!string.IsNullOrEmpty(lastUpdateFileDir))
                    Directory.CreateDirectory(lastUpdateFileDir);
                File.WriteAllText(options.LastUpdateFilePath, content);
                logger.Info("write LastUpdate to " + options.LastUpdateFilePath);
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
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percent, null));
        }
    }
}