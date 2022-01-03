using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdateServer.Services
{
    public interface IUpdateService
    {
        Task<UpdateFileCollection> ScanFiles();
        Task<UpdateFileCollection> UpdateFiles(UpdateFileCollection updateFiles);
    }
}