﻿using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AlphabetUpdateServer.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "manager")]
    public class LauncherController : ControllerBase
    {
        private readonly ILogger<LauncherController> logger;

        private readonly string launcherInfoPath;
        private readonly string filesPath;
        private readonly string launcherCachePath;

        public LauncherController(
            ILogger<LauncherController> _logger, 
            IWebHostEnvironment _env, 
            IOptions<UpdateFileOptions> _options)
        {
            logger = _logger;
            IWebHostEnvironment env = _env;
            UpdateFileOptions options = _options.Value;
            
            launcherInfoPath = Path.Combine(env.WebRootPath, options.LauncherInfoPath);
            filesPath = Path.Combine(env.WebRootPath, options.FilesCachePath);
            launcherCachePath = Path.Combine(env.WebRootPath, "launcher.json");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var json = await System.IO.File.ReadAllTextAsync(launcherCachePath);
            return Ok(json);
        }

        [HttpGet("files")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUpdate()
        {
            if (!System.IO.File.Exists(filesPath))
                return Problem("no files");
            
            var json = await System.IO.File.ReadAllTextAsync(filesPath);
            return Ok(json);
        }
        
        [HttpPost("files")]
        public async Task<IActionResult> PostUpdate(UpdateFileCollection updateFiles)
        {
            if (string.IsNullOrEmpty(updateFiles.HashAlgorithm))
                return BadRequest("No HashAlgorithm");

            if (updateFiles.Files == null)
                return BadRequest("No Files");
            
            updateFiles.LastUpdate = DateTime.Now;

            await Util.WriteJson(filesPath, updateFiles);
            var json = await updateCache(null, updateFiles);
            logger.LogInformation("Update file collection");
            return Ok(json);
        }

        [HttpGet("info")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInfo()
        {
            if (!System.IO.File.Exists(launcherInfoPath))
                return Problem("no launcher info file");

            var json = await Util.ReadJson<LauncherInfo>(launcherInfoPath);
            return Ok(json);
        }

        [HttpPost("info")]
        public async Task<IActionResult> PostInfo(LauncherInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
                return BadRequest("No Name");
            if (string.IsNullOrEmpty(info.LauncherServer))
                return BadRequest("No LauncherServer");
            if (string.IsNullOrEmpty(info.StartVersion))
                return BadRequest("No StartVersion");
            
            await Util.WriteJson(launcherInfoPath, info);
            var json = await updateCache(info, null);
            logger.LogInformation("Update launcher info");
            return Ok(json);
        }

        [HttpDelete("info")]
        public IActionResult DeleteInfo()
        {
            if (!System.IO.File.Exists(launcherInfoPath))
                return Problem("no launcher info file");

            System.IO.File.Delete(launcherInfoPath);
            logger.LogInformation("delete launcher info file: {Path}", launcherInfoPath);
            return NoContent();
        }

        private async Task<string> updateCache(LauncherInfo? launcherInfo, UpdateFileCollection? updateFiles)
        {
            if (launcherInfo == null)
            {
                if (System.IO.File.Exists(launcherInfoPath))
                    launcherInfo = await Util.ReadJson<LauncherInfo>(launcherInfoPath);
                else
                    launcherInfo = new LauncherInfo();
            }

            if (updateFiles == null)
            {
                if (System.IO.File.Exists(filesPath))
                    updateFiles = await Util.ReadJson<UpdateFileCollection>(filesPath);
                else
                    updateFiles = new UpdateFileCollection();
            }

            var obj = new
            {
                LastUpdate = DateTime.Now,
                Launcher = launcherInfo,
                Files = updateFiles
            };

            var json = await Util.WriteJson(launcherCachePath, obj);
            return json;
        }
    }
}
