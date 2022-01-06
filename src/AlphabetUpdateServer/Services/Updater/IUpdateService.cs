using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdateServer.Services.Updater
{
    public interface IUpdateService
    {
        Task<UpdateFileCollection> Update(UpdateFileCollection updateFiles);
    }
}