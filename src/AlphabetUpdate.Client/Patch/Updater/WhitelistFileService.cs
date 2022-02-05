using AlphabetUpdate.Client.Patch;
using AlphabetUpdate.Client.Patch.Service;
using AlphabetUpdate.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Updater
{
    public class WhitelistFileService : PatchServiceBase<WhitelistFileSetting>, IWhitelistFileService
    {
        private readonly HashSet<string> _whitelistFiles = new HashSet<string>();
        private readonly List<string> _whitelistDirs = new List<string>();

        public override Task Initialize()
        {
            if (Setting?.WhitelistFilePath != null)
            {
                foreach (var filepath in Setting.WhitelistFilePath)
                {
                    AddWhitelistFilePath(filepath);
                }
            }

            if (Setting?.WhitelistDirPath != null)
            {
                foreach (var dirpath in Setting.WhitelistDirPath)
                {
                    AddWhitelistDirPath(dirpath);
                }
            }

            return Task.CompletedTask;
        }

        private string normalizePath(string filepath)
        {
            if (!Path.IsPathFullyQualified(filepath))
                filepath = Path.Combine(PatchContext?.BasePath, filepath);
            filepath = IoHelper.NormalizePath(filepath, fullPath: true);

            return filepath.ToLowerInvariant();
        }

        public void AddWhitelistFilePath(string path)
        {
            path = normalizePath(path);
            _whitelistFiles.Add(path);
        }

        public void AddWhitelistDirPath(string path)
        {
            path = normalizePath(path);
            _whitelistDirs.Add(path);
        }

        public bool CheckWhitelistFile(string path)
        {
            var normalizedPath = normalizePath(path);
            return _whitelistFiles.Contains(normalizedPath);
        }

        public bool CheckWhitelistDir(string path)
        {
            var normalizedPath = normalizePath(path);

            foreach (var dirPath in _whitelistDirs)
            {
                if (normalizedPath.StartsWith(dirPath))
                    return true;
            }

            return false;
        }
    }
}
