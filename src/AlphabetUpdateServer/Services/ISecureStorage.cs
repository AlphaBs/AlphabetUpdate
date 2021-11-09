using System.Threading.Tasks;
using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServer.Services
{
    public interface ISecureStorage
    {
        SecureKeys GetObject();
        Task<SecureKeys> Load();
        Task Save(SecureKeys obj);
    }
}
