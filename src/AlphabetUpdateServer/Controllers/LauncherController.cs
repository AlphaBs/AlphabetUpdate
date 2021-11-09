using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace AlphabetUpdateServer.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "manager")]
    public class LauncherController : ControllerBase
    {
        private readonly ILogger<LauncherController> logger;

        private readonly UpdateFileOptions options;
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
            options = _options.Value;
            
            var webRoot = env.WebRootPath ?? env.ContentRootPath ?? "./";
            launcherInfoPath = Path.Combine(webRoot, options.LauncherInfoPath);
            filesPath = Path.Combine(webRoot, options.FilesCachePath);
            launcherCachePath = Path.Combine(webRoot, "launcher.json");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            if (!System.IO.File.Exists(launcherCachePath))
                return Problem("no cache");
                
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

            if (!Directory.Exists(options.OutputDir))
                Directory.CreateDirectory(options.OutputDir);
            var outFilesArr = Directory.GetFiles(options.OutputDir, "*.*", SearchOption.AllDirectories);
            var outFilesSet = new HashSet<string>(outFilesArr);

            foreach (var inFile in updateFiles.Files)
            {
                if (string.IsNullOrEmpty(inFile.Path))
                    return BadRequest("One of files does not have a path");

                var path = normalizePath(inFile.Path);
                var realPath = Path.Combine(options.InputDir, path);

                if (!System.IO.File.Exists(realPath))
                    return BadRequest("Cannot find file: " + inFile.Path);

                var outFilePath = Path.Combine(options.OutputDir, path);
                var outFileDir = Path.GetDirectoryName(outFilePath);
                if (!string.IsNullOrEmpty(outFileDir) && !Directory.Exists(outFileDir))
                    Directory.CreateDirectory(outFileDir);
                
                System.IO.File.Copy(realPath, outFilePath, overwrite: true);
                logger.LogInformation("Copy update file: {Input} -> {Output}", realPath, outFilePath);
                outFilesSet.Remove(outFilePath.ToLowerInvariant());
            }

            foreach (var remainFile in outFilesSet)
            {
                System.IO.File.Delete(remainFile);
                logger.LogInformation("Delete remain file: {Name}", remainFile);
            }
            
            Util.DeleteEmptyDirs(options.OutputDir);
            
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
        
        private string normalizePath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Trim(Path.DirectorySeparatorChar);
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
