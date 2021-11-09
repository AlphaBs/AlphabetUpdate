using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
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
            logger.LogInformation("InputDir: {InputDir}, OutputDir: {OutputDir}, BaseUrl: {BaseUrl}",
                options.InputDir, options.OutputDir, options.BaseUrl);

            var updateFileGenerator = new UpdateFileGenerator(
                options.InputDir,
                options.OutputDir);
            var files = updateFileGenerator.GetUpdateFiles();

            var obj = new UpdateFileCollection
            {
                LastUpdate = DateTime.Now,
                HashAlgorithm = "md5",
                Files = files
            };

            var json = JsonSerializer.Serialize(obj, JsonHelper.JsonOptions);
            return Ok(json);
        }
    }
}
