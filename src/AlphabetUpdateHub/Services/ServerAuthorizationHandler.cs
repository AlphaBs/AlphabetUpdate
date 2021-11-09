using System.Threading.Tasks;
using AlphabetUpdateHub.Models;
using Microsoft.AspNetCore.Authorization;

namespace AlphabetUpdateHub.Services
{
    public class ServerAuthorizationHandler : AuthorizationHandler<ServerIdRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ServerIdRequirement requirement)
        {
            
        }
    }
}