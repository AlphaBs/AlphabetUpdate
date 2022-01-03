using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;

namespace AlphabetUpdateServer.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "manager")]
    public class LauncherController : ControllerBase
    {
        private readonly ILogger<LauncherController> logger;
        private readonly ILauncherService launcher;
        private readonly IUpdateService updater;
        
        public LauncherController(
            ILogger<LauncherController> _logger, 
            ILauncherService launcherService,
            IUpdateService updateService)
        {
            this.logger = _logger;
            this.launcher = launcherService;
            this.updater = updateService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await launcher.GetCache());
        }

        [HttpGet("files")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUpdate()
        {
            return Ok(await launcher.GetFilesCache());
        }
        
        [HttpPost("files")]
        public async Task<IActionResult> PostUpdate(UpdateFileCollection? updateFiles)
        {
            if (string.IsNullOrEmpty(updateFiles?.HashAlgorithm))
                return BadRequest("No HashAlgorithm");

            if (updateFiles?.Files == null)
                return BadRequest("No Files");

            updateFiles = await updater.UpdateFiles(updateFiles);
            return Ok(await launcher.UpdateFiles(updateFiles));
        }

        [HttpGet("info")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInfo()
        {
            return Ok(await launcher.GetInfoCache());
        }

        [HttpPost("info")]
        public async Task<IActionResult> PostInfo(LauncherInfo info)
        {
            return Ok(await launcher.UpdateInfo(info));
        }

        [HttpDelete("info")]
        public async Task<IActionResult> DeleteInfo()
        {
            await launcher.DeleteInfo();
            return NoContent();
        }
    }
}
