using System;
using System.Linq;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlphabetUpdateServer.Pages.Manager
{
    [Authorize(Roles = "manager")]
    public class Index : PageModel
    {
        private readonly ILauncherService launcher;
        private readonly ILogger<Index> logger;
        private readonly UpdateFileOptions updateFileOptions;

        public Index(
            ILauncherService launcherService,
            ILogger<Index> log,
            IOptions<UpdateFileOptions> opts)
        {
            launcher = launcherService;
            logger = log;
            updateFileOptions = opts.Value;
        }

        public string? Status;
        public string? Message;
        public LauncherInfo? Info;
        public UpdateFileCollection? Files;
        
        private LauncherInfo createDefaultLauncherInfo()
        {
            return new LauncherInfo
            {
                Name = updateFileOptions.Name,
                LauncherServer = updateFileOptions.BaseUrl,
                WhitelistFiles = new string[]
                {
                    "options.txt",
                    "optionsof.txt",
                    "optionsshader.txt"
                },
                WhitelistDirs = new string[]
                {
                    "saves",
                    "screenshots",
                    "journeymap"
                }
            };
        }

        private UpdateFileCollection createDefaultFileCollection()
        {
            return new UpdateFileCollection
            {
                HashAlgorithm = "md5"
            };
        }

        private async Task setLauncherInfoForView()
        {
            try
            {
                Info = await launcher.GetInfo();
            }
            catch (Exception e)
            {
                this.Status += "\nInfoError: " + e.Message;
            }

            if (Info == null)
                Info = createDefaultLauncherInfo();
        }

        private async Task setUpdateFilesForView()
        {
            try
            {
                Files = await launcher.GetFiles();
            }
            catch (Exception e)
            {
                this.Status += "\nFilesError: " + e.Message;
            }


            if (Files == null)
                Files = createDefaultFileCollection();
        }

        public async Task<IActionResult> OnGetAsync(string? status, string? message)
        {
            this.Status = "";

            await setLauncherInfoForView();
            await setUpdateFilesForView();

            this.Status += status;
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
                await setLauncherInfoForView();
                Message = "업데이트 실패: " + e.Message;
            }
            
            await setUpdateFilesForView();
            return Page();
        }
    }
}