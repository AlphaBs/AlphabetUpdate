using System;
using System.Linq;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateServer.Pages.Manager
{
    [Authorize(Roles = "manager")]
    public class Index : PageModel
    {
        private readonly ILauncherService launcher;
        private readonly ILogger<Index> logger;

        public Index(
            ILauncherService launcherService,
            ILogger<Index> log)
        {
            launcher = launcherService;
            logger = log;
        }

        public string? Status;
        public string? Message;
        public LauncherInfo? Info;
        public UpdateFileCollection? Files;
        
        public async Task<IActionResult> OnGetAsync(string? status, string? message)
        {
            try
            {
                Info = await launcher.GetInfo();
                Files = await launcher.GetFiles();
            }
            catch (Exception e)
            {
                
            }

            this.Status = status;
            this.Message = message;
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateInfoAsync(
            string name,
            string? gameServerIp,
            string? startVersion,
            string? startVanillaVersion,
            string? launcherServer,
            string? whitelistFiles,
            string? whitelistDirs)
        {
            try
            {
                var info = new LauncherInfo()
                {
                    Name = name,
                    GameServerIp = gameServerIp,
                    StartVersion = startVersion,
                    StartVanillaVersion = startVanillaVersion,
                    LauncherServer = launcherServer,
                    WhitelistFiles = (whitelistFiles ?? "")
                        .Split('\n')
                        .Select(x => x.Trim())
                        .ToArray(),
                    WhitelistDirs = (whitelistDirs ?? "")
                        .Split('\n')
                        .Select(x => x.Trim())
                        .ToArray()
                };

                await launcher.UpdateInfo(info);
                Info = info;
                Status = "런처 정보 업데이트 완료!";
                Message = "업데이트 완료";
            }
            catch (ArgumentException e)
            {
                Info = await launcher.GetInfo();
                Message = "업데이트 실패: " + e.Message;
            }
            
            Files = await launcher.GetFiles();
            return Page();
        }
    }
}