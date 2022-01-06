using System.Threading.Tasks;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AlphabetUpdateServer.Services
{
    public static class JwtTokenValidator
    {
        public static Task ValidateAudience(TokenValidatedContext context)
        {
            var remoteClientAddressResolver = new RemoteClientAddressResolver();
            var host = remoteClientAddressResolver.Resolve(context.HttpContext);

            var valid = context.Principal?.HasClaim("aud", host ?? "") ?? false;
            if (!valid)
                context.Fail("Invalid aud");

            return Task.CompletedTask;
        }
    }
}