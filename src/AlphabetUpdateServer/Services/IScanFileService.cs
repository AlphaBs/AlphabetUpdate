using AlphabetUpdate.Common.Models;
using System.Threading.Tasks;

namespace AlphabetUpdateServer.Services
{
    public interface IScanFileService
    {
        Task<UpdateFileCollection> ScanFile();
    }
}
