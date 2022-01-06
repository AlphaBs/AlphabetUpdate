using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Updater;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateServer.Pages.Manager
{
    public class Update : PageModel
    {
        private readonly ILogger<Update> logger;
        private readonly IScanFileService scanner;
        private readonly IEnumerable<IUpdateService> updaters;
        
        public Update(
            ILogger<Update> log,
            IScanFileService scanService,
            IEnumerable<IUpdateService> updateServices)
        {
            logger = log;
            updaters = updateServices;
            scanner = scanService;
        }

        public UpdateFileCollection? Files;
        
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return Page();
        }
    }
}