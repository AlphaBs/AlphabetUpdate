using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Services
{
    public interface IFileEnabler
    {
        Task<bool> CanEnable(string path);
        Task<bool> CanDisable(string path);

        Task EnableFile(string path);
        Task DisableFile(string path);
    }
}
