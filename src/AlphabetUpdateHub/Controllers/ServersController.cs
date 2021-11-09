using System;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.Services;
using AlphabetUpdateHub.UpdateServer;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateHub.Controllers
{
    [Route("servers")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly AlphabetUpdateServerService updateServerService;
        private readonly LauncherCacheService launcherCacheService;

        public ServersController(LauncherCacheService cacheService, AlphabetUpdateServerService serverService)
        {
            this.updateServerService = serverService;
            this.launcherCacheService = cacheService;
        }

        private async Task<LauncherMetadata?> GetMetadataCache(string serverId)
        {
            var cache = await launcherCacheService.GetMetadata(serverId);
            if (cache == null)
            {
                var server = await updateServerService.GetByServerId(serverId);
                if (server == null)
                    return null;
            }

            return cache;
        }
        
        [HttpGet("{serverId}")]
        public async Task<IActionResult> GetServer(string serverId)
        {
            var l = await GetMetadataCache(serverId);
            if (l == null)
                return NotFound();
            
            return Ok(l);
        }

        [HttpGet("{serverId}/info")]
        public async Task<IActionResult> GetServerInfo(string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
                return NotFound();
            
            var l = await GetMetadataCache(serverId);
            if (l == null)
                return NotFound();
            
            return Ok(l.Launcher);
        }

        [HttpPost("{serverId}/info")]
        public async Task<IActionResult> PostServerInfo(string serverId, LauncherInfo? info)
        {
            if (info == null)
                return BadRequest();
            
            var server = await updateServerService.GetByServerId(serverId);
            if (server == null)
                return NotFound();
            
            var api = new AlphabetUpdateServerApi(server);
            var metadata = await api.UpdateLauncherInfo(info);

            if (metadata == null)
                return Problem();

            await launcherCacheService.CreateOrUpdate(LauncherMetadataCache.Create(serverId, metadata));

            return Ok(metadata);
        }
        
        [HttpGet("{serverId}/files")]
        public async Task<IActionResult> GetServerFiles(string serverId)
        {
            var l = await GetMetadataCache(serverId);
            if (l == null)
                return NotFound();
            
            return Ok(l.Files);
        }

        [HttpPost("{serverId}/files")]
        public async Task<IActionResult> PostServerFiles(string serverId, UpdateFileCollection? files)
        {
            if (files == null)
                return BadRequest();
            
            var server = await updateServerService.GetByServerId(serverId);
            if (server == null)
                return NotFound();
            
            var api = new AlphabetUpdateServerApi(server);
            var metadata = await api.UpdateLauncherFiles(files);

            if (metadata == null)
                return Problem();

            await launcherCacheService.CreateOrUpdate(LauncherMetadataCache.Create(serverId, metadata));

            return Ok(metadata);
        }
    }
}