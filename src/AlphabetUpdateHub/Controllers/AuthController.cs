using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AlphabetUpdateHub.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly AuthOptions authOption;
        private readonly AccountService accountService;

        public AuthController(
            ILogger<AuthController> logger,
            IOptions<AuthOptions> auth,
            AccountService account)
        {
            this.logger = logger;
            this.authOption = auth.Value;
            this.accountService = account;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCredentials? credentials)
        {
            if (string.IsNullOrEmpty(credentials?.Id) ||
                string.IsNullOrEmpty(credentials?.Password))
            {
                return BadRequest();
            }
            
            logger.LogInformation("login attempt: {Name}", credentials.Id);
            
            var account = await accountService.GetById(credentials.Id);
            var reqPassword = credentials.Password + account.Salt;
            var validPassword = BCrypt.Net.BCrypt.Verify(reqPassword, account.HashedPassword);
            if (!validPassword)
                return Unauthorized();

            var jwt = generateJwt(account.Id, account.Servers);
            if (string.IsNullOrEmpty(jwt))
                return Unauthorized();

            return Ok(new
            {
                Token = jwt
            });
        }

        private string? generateJwt(string id, string[] servers)
        {
            if (string.IsNullOrEmpty(authOption.SecretKey))
            {
                logger.LogError("null authOption.SecretKey");
                return null;
            }
            
            try
            {
                var key = new SymmetricSecurityKey(Convert.FromBase64String(authOption.SecretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: authOption.Issuer,
                    claims: new Claim[]
                    {
                        new Claim("id", id),
                        new Claim("s", string.Join(',', servers))
                    },
                    expires: DateTime.Now.AddMinutes(authOption.ExpiresM),
                    notBefore: DateTime.Now,
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError("generateJwt: {Ex}", ex);
                throw;
            }
        }
    }
}