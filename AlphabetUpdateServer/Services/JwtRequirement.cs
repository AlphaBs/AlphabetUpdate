using Microsoft.AspNetCore.Authorization;

namespace AlphabetUpdateServer.Services
{
    public class JwtRequirement : IAuthorizationRequirement
    {
        public JwtRequirement(string sub)
        {
            this.Subject = sub;
        }
        
        public string Subject { get; set; }
    }
}