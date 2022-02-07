using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Services
{
    public interface IWhitelistFileService
    {
        void AddWhitelistFilePath(string path);
        void AddWhitelistDirPath(string path);
        bool CheckWhitelistFile(string path);
        bool CheckWhitelistDir(string path);
    }
}
