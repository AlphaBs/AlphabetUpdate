using AlphabetUpdate.Client.Patch.Handler;
using AlphabetUpdate.Common.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Updater
{
    public class FileFilterHandler : PatchHandlerBase<FileFilterSetting>
    {
        private readonly ILogger<FileFilterHandler> _logger;
        private readonly IWhitelistFileService _whitelistService;
        private readonly IFileTagService _fileTagService;

        public FileFilterHandler(
            ILogger<FileFilterHandler> logger,
            IWhitelistFileService whitelistService,
            IFileTagService fileTagService)
        {
            _logger = logger;
            _whitelistService = whitelistService;
            _fileTagService = fileTagService;
        }

        public override async Task Patch(CancellationToken? cancellationToken)
        {
            if ()
            {
                await FilterFiles(PatchContext.BasePath);
            }
            else if (Setting?.AlwaysFilter != null)
            {
                // keep always up-to-date
                foreach (var item in Setting.AlwaysFilter)
                {
                    var path = Path.Combine(PatchContext.BasePath, item);
                    await FilterFiles(path);
                }
            }
        }

        public async Task FilterFiles(string targetPath)
        {
            _logger.LogInformation("FilterFiles: {Path}", targetPath);

            if (!Directory.Exists(targetPath))
                return;

            var f = GetTargetFiles(targetPath)
                .Except(updateFileCollection.Files.Select(x
                    => IoHelper.NormalizePath(Path.Combine(PatchContext.BasePath, x.Path))));

            f = f.Where(x => !_whitelistService.CheckWhitelistFile(x));

            int count = 0;
            foreach (var path in f)
            {
                File.Delete(path);
                count++;
            }

            _logger.LogInformation("{DeletedFileCount} files were deleted", count);
        }

        public List<string> GetTargetFiles(string path)
        {
            var list = new List<string>();
            GetTargetFiles(path, list);
            return list;
        }

        private void GetTargetFiles(string dir, List<string> results)
        {
            var directoryInfo = new DirectoryInfo(dir);
            var files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                .Select(x => x.FullName);

            results.AddRange(files);

            var dirs = directoryInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
            foreach (var item in dirs)
            {
                var realPath = Path.Combine(dir, item.Name);
                if (_whitelistService != null && _whitelistService.CheckWhitelistDir(realPath))
                    continue;
                GetTargetFiles(realPath, results);
            }
        }
    }
}
