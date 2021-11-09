using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdate.Client.UpdateServer
{
    public interface IUpdateServerApi
    {
        Task<LauncherMetadata?> GetLauncherMetadata();
        Task<LauncherInfo?> GetLauncherInfo();
        Task<UpdateFileCollection?> GetUpdateFileCollection();
    }
}