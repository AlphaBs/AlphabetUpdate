using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AlphabetUpdateServer.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize(Policy="manager")]
    public class UpdateFileController : ControllerBase
    {
        private readonly ILogger<UpdateFileController> logger;
        private readonly IWebHostEnvironment env;
        private readonly UpdateFileOptions options;

        public UpdateFileController(
            ILogger<UpdateFileController> _logger, 
            IWebHostEnvironment _env, 
            IOptions<UpdateFileOptions> _options)
        {
            logger = _logger;
            env = _env;
            options = _options.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var basePath = options.Root;
            if (string.IsNullOrEmpty(basePath))
                basePath = env.WebRootPath;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var updateFilePath = options.Path;

            logger.LogInformation("basePath: {BasePath}, baseUrl: {BaseUrl}, updatefilePath: {UpdateFilePath}",
                basePath, baseUrl, updateFilePath);

            if (string.IsNullOrEmpty(basePath))
                return Problem("invalid basePath");

            if (string.IsNullOrEmpty(baseUrl))
                return Problem("invalid baseUrl");

            var updateFileGenerator = new UpdateFileGenerator(basePath, baseUrl, updateFilePath);
            var files = updateFileGenerator.GetTagUpdateFiles();

            var obj = new UpdateFileCollection
            {
                LastUpdate = DateTime.Now,
                HashAlgorithm = "md5",
                Files = files
            };

            var json = JsonSerializer.Serialize(obj);
            return Ok(json);
        }
    }
}
