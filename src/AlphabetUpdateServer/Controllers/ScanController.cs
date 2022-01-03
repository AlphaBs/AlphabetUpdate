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
        private readonly IUpdateService updater;
        private readonly ILogger<ScanController> logger;
        
        public ScanController(
            IUpdateService updateService,
            ILogger<ScanController> log)
        {
            updater = updateService;
            logger = log;
        }
        
        public async Task<IActionResult> Get()
        {
            var files = await updater.ScanFiles();
            var json = JsonSerializer.Serialize(files, JsonHelper.JsonOptions);
            return Ok(json);
        }
    }
}