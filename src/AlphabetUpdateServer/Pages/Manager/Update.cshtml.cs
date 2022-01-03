using System;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AlphabetUpdateServer.Pages.Manager
{
    public class Update : PageModel
    {
        private readonly ILogger<Update> logger;
        private readonly IUpdateService updater;
        
        public Update(
            ILogger<Update> log,
            IUpdateService updateService)
        {
            logger = log;
            updater = updateService;
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
                Files = await updater.ScanFiles();
                Files = await updater.UpdateFiles(Files);
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