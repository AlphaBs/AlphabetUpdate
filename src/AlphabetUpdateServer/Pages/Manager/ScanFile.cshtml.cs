using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AlphabetUpdateServer.Pages.Manager
{
    public class ScanFileModel : PageModel
    {
        private readonly IScanFileService scanner;

        public ScanFileModel(
            IScanFileService scanService)
        {
            scanner = scanService;
        }

        public UpdateFileCollection? Files;

        public async Task OnPostAsync()
        {
            var files = await scanner.ScanFile();
            Files = files;
        }
    }
}
