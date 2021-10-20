using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AlphabetUpdateServer.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly AuthOptions authOption;
        private readonly SecureKeys secureKeys;

        public AuthController(
            ILogger<AuthController> logger,
            IOptions<AuthOptions> authOptions,
            ISecureStorage ss)
        {
            this.logger = logger;
            authOption = authOptions.Value;
            secureKeys = ss.GetObject();
        }

        [HttpPost("login")]
        [RequestSizeLimit(1024)]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            var buffer = new byte[Request.ContentLength ?? 0];
            await Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var encryptedStr = Encoding.UTF8.GetString(buffer);
            
            if (string.IsNullOrEmpty(encryptedStr))
                return ValidationProblem("no encryptedKey");

            var encryptedData = Convert.FromBase64String(encryptedStr);
            
            var loginModel = await decryptAesKey(encryptedData);
            if (loginModel == null)
                return Unauthorized();

            logger.LogInformation("login attempt: {Name}, {Host}", loginModel.Name, loginModel.Host);

            if (string.IsNullOrEmpty(loginModel.Name))
                return Unauthorized();

            var remoteAddress = Request.HttpContext.Connection.RemoteIpAddress;
            if (remoteAddress == null)
                return Unauthorized();

            if (remoteAddress.ToString() != loginModel.Host && !IPAddress.IsLoopback(remoteAddress))
                return Unauthorized();

            var reqPassword = loginModel.Password + secureKeys.Salt;
            var validPassword = BCrypt.Net.BCrypt.Verify(reqPassword, secureKeys.HashedPassword);
            if (!validPassword)
                return Unauthorized();
            
            var jwt = generateJwt(loginModel.Name);
            if (string.IsNullOrEmpty(jwt))
                return Unauthorized();

            logger.LogInformation("new jwt: {Name}", loginModel.Name);

            return Ok(new 
            {
                Token = jwt
            });
        }

        private async Task<LoginModel?> decryptAesKey(byte[] encryptedKey)
        {
            if (string.IsNullOrEmpty(secureKeys.AesKey) || string.IsNullOrEmpty(secureKeys.AesIV))
            {
                logger.LogError("null secureKeys.Aes");
                return null;
            }

            try
            {
                await using var ms = new MemoryStream(encryptedKey);
                var aes = new AesWrapper(secureKeys.AesKey, secureKeys.AesIV);
                return await aes.AesDecrypt<LoginModel>(ms);
            }
            catch (Exception ex)
            {
                logger.LogError("decryptAesKey error: {Ex}", ex);
                return null;
            }
        }

        private string? generateJwt(string name)
        {
            if (string.IsNullOrEmpty(secureKeys.SecretKey))
            {
                logger.LogError("null secureKeys.SecretKey");
                return null;
            }
            
            try
            {
                var key = new SymmetricSecurityKey(Convert.FromBase64String(secureKeys.SecretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: authOption.Issuer,
                    audience: Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    claims: new Claim[]
                    {
                        new Claim("r", "manager"),
                        new Claim("s", name)
                    },
                    expires: DateTime.Now.AddMinutes(authOption.ExpiresM),
                    notBefore: DateTime.Now,
                    signingCredentials: credentials);


                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError("generateJwt: {Ex}", ex);
                return null;
            }
        }
    }
}
