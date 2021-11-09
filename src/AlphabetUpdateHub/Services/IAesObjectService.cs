using System.IO;
using System.Threading.Tasks;

namespace AlphabetUpdateHub.Services
{
    public interface IAesObjectService
    {
        Task AesEncrypt(object? obj, Stream writeTo);
        Task<T?> AesDecrypt<T>(Stream stream);
    }
}