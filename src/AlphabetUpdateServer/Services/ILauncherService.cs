using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdateServer.Services
{
    public interface ILauncherService
    {
        Task<string> GetCache();
        Task<string> Update(LauncherInfo? launcherInfo, UpdateFileCollection? updateFiles);

        Task<UpdateFileCollection> GetFiles();
        Task<string> GetFilesCache();
        Task<string> UpdateFiles(UpdateFileCollection updateFiles);
        Task DeleteFiles();

        Task<LauncherInfo> GetInfo();
        Task<string> GetInfoCache();
        Task<string> UpdateInfo(LauncherInfo info);
        Task DeleteInfo();
    }
}