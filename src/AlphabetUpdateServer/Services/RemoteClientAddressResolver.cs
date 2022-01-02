using Microsoft.AspNetCore.Http;

namespace AlphabetUpdateServer.Services
{
    public class RemoteClientAddressResolver : IClientAddressResolver
    {
        public string? Resolve(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}