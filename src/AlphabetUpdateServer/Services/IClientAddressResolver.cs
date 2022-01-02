using Microsoft.AspNetCore.Http;

namespace AlphabetUpdateServer.Services
{
    public interface IClientAddressResolver
    {
        string? Resolve(HttpContext context);
    }
}