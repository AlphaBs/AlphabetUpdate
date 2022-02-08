using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Updater;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateServer.Pages.Manager
{
    public class Update : PageModel
    {
        private readonly ILogger<Update> logger;
        private readonly IScanFileService scanner;
        private readonly IEnumerable<IUpdateService> updaters;
        private readonly ILauncherService launcher;

        public Update(
            ILogger<Update> log,
            IScanFileService scanService,
            IServiceProvider serviceProvider, 
            ILauncherService launcher)
        {
            logger = log;
            updaters = serviceProvider.GetServices<IUpdateService>();
            scanner = scanService;
            this.launcher = launcher;
        }

        public UpdateFileCollection? Files;
        public string? Message;
        public string? DetailedMessage;
        
        public void OnGet()
        {
            Redirect("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Files = await scanner.ScanFile();

                foreach (var updater in updaters)
                {
                    Files = await updater.Update(Files);
                }

                await launcher.UpdateFiles(Files);
            }
            catch (Exception e)
            {
                Message = "업데이트 실패! 오류가 발생했습니다";
                DetailedMessage = e.ToString();
            }
            
            return Page();
        }
    }
}