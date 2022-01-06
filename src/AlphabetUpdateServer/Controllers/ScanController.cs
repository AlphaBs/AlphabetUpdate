using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateServer.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize(Roles = "manager")]
    public class ScanController : ControllerBase
    {
        private readonly IScanFileService scanner;
        private readonly ILogger<ScanController> logger;
        
        public ScanController(
            IScanFileService scanService,
            ILogger<ScanController> log)
        {
            scanner = scanService;
            logger = log;
        }
        
        public async Task<IActionResult> Get()
        {
            var files = await scanner.ScanFile();
            var json = JsonSerializer.Serialize(files, JsonHelper.JsonOptions);
            return Ok(json);
        }
    }
}