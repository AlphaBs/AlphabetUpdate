using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using AlphabetUpdateServer.Services.Updater;
using System.Collections.Generic;

namespace AlphabetUpdateServer.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "manager")]
    public class LauncherController : ControllerBase
    {
        private readonly ILogger<LauncherController> logger;
        private readonly ILauncherService launcher;
        private readonly IEnumerable<IUpdateService> updaters;
        
        public LauncherController(
            ILogger<LauncherController> _logger, 
            ILauncherService launcherService,
            IEnumerable<IUpdateService> updateServices)
        {
            this.logger = _logger;
            this.launcher = launcherService;
            this.updaters = updateServices;
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

            foreach (var updater in updaters)
            {
                updateFiles = await updater.Update(updateFiles);
            }
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
