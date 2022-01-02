using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlphabetUpdateServer.BasicAuth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
    {
        private readonly IUserService userService;
        
        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService user) :
            base(options, logger, encoder, clock)
        {
            userService = user;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                string authHeader = Context.Request.Headers["Authorization"];
                if (string.IsNullOrWhiteSpace(authHeader))
                    return Task.FromResult(AuthenticateResult.NoResult());
                
                var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                if (!authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(),
                        StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult(AuthenticateResult.NoResult());
                
                var credentials = Encoding.UTF8
                    .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                    .Split(':', 2);
                if (credentials.Length != 2)
                    return Task.FromResult(AuthenticateResult.NoResult());

                var username = credentials[0];
                var password = credentials[1];
                var user = userService.Authenticate(username, password);
                if (user == null)
                    return Task.FromResult(AuthenticateResult.Fail("No user"));

                var cp = new ClaimsPrincipal();
                cp.AddIdentity(new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role ?? "")
                }));

                var ticket = new AuthenticationTicket(cp, BasicAuthenticationScheme.AuthenticationScheme);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (FormatException e)
            {
                return Task.FromResult(AuthenticateResult.Fail(e));
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var headerValue = "Basic";
            if (!string.IsNullOrWhiteSpace(Options.Realm))
                headerValue += $" realm={Options.Realm}";
            Response.Headers["WWW-Authenticate"] = headerValue;
            Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            return base.HandleForbiddenAsync(properties);
        }
    }
}