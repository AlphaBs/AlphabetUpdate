using System;
using System.Linq;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.Services;
using AlphabetUpdateHub.UpdateServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateHub.Controllers
{
    [Route("servers")]
    [ApiController]
    [Authorize]
    public class ServersController : ControllerBase
    {
        private readonly LauncherMetadataCacheService launcherMetadataCacheService;
        private readonly UpdateServerMetadataService updateServerMetadataService;
        private readonly ILogger<ServersController> logger;
        
        public ServersController(
            ILogger<ServersController> _logger,
            LauncherMetadataCacheService cacheService, 
            UpdateServerMetadataService metadataService)
        {
            this.launcherMetadataCacheService = cacheService;
            this.updateServerMetadataService = metadataService;
            this.logger = _logger;
        }

        private async Task<LauncherMetadata?> GetFreshLauncherMetadata(string serverId)
        {
            var cache = await launcherMetadataCacheService.GetByServerId(serverId);
            
            if (cache?.LauncherMetadata == null || cache.LastMetadataUpdate.AddHours(24) > DateTime.Now)
            {
                var server = await updateServerMetadataService.GetByServerId(serverId);
                if (server?.UpdateServer == null)
                    return null;
                
                var newMetadata = await launcherMetadataCacheService.UpdateLauncherMetadata(server.UpdateServer);
                return newMetadata;
            }

            return cache.LauncherMetadata;
        }
        
        private string[] GetServers()
        {
            return User.FindFirst("s")?.Value?.Split(',') ?? Array.Empty<string>();
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var servers = GetServers();
            var result = await updateServerMetadataService.GetServers(servers);

            return Ok(result);
        }

        [HttpGet("{serverId}")]
        public async Task<IActionResult> GetServer(string serverId)
        {
            var l = await GetFreshLauncherMetadata(serverId);
            if (l == null)
                return NotFound();
            
            return Ok(l);
        }

        [HttpGet("{serverId}/info")]
        public async Task<IActionResult> GetServerInfo(string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
                return NotFound();
            
            var l = await GetFreshLauncherMetadata(serverId);
            if (l == null)
                return NotFound();
            
            return Ok(l.Launcher);
        }

        [HttpPost("{serverId}/info")]
        public async Task<IActionResult> PostServerInfo(string serverId, LauncherInfo? info)
        {
            if (info == null)
                return BadRequest();
         
            if (!GetServers().Contains(serverId))
                return Forbid();
            
            var server = await updateServerMetadataService.GetByServerId(serverId);
            if (server?.UpdateServer == null)
                return NotFound();
            
            var api = new AlphabetUpdateServerApi(server.UpdateServer);
            var metadata = await api.UpdateLauncherInfo(info);

            if (metadata == null)
                return Problem();

            await launcherMetadataCacheService.CreateOrUpdate(
                LauncherMetadataCache.Create(serverId, metadata));

            return Ok(metadata);
        }
        
        [HttpGet("{serverId}/files")]
        public async Task<IActionResult> GetServerFiles(string serverId)
        {
            var l = await GetFreshLauncherMetadata(serverId);
            if (l == null)
                return NotFound();
            
            return Ok(l.Files);
        }

        [HttpPost("{serverId}/files")]
        public async Task<IActionResult> PostServerFiles(string serverId, UpdateFileCollection? files)
        {
            if (files == null)
                return BadRequest();

            if (!GetServers().Contains(serverId))
                return Forbid();
            
            var server = await updateServerMetadataService.GetByServerId(serverId);
            if (server?.UpdateServer == null)
                return NotFound();
            
            var api = new AlphabetUpdateServerApi(server.UpdateServer);
            var metadata = await api.UpdateLauncherFiles(files);

            if (metadata == null)
                return Problem();

            await launcherMetadataCacheService.CreateOrUpdate(
                LauncherMetadataCache.Create(serverId, metadata));

            return Ok(metadata);
        }
    }
}