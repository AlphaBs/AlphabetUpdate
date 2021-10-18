using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AlphabetUpdateServer.Services
{
    public class JwtAuthorizationHandler : AuthorizationHandler<JwtRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, JwtRequirement requirement)
        {
            var host = "";
            if (context.Resource is HttpContext httpContext)
            {
                host = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            var audResult = context.User.HasClaim("aud", host ?? "");
            var roleResult = context.User.HasClaim("r", requirement.Subject);
            
            if (audResult && roleResult)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}