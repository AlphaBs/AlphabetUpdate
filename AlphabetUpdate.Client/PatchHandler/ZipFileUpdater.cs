using System.ComponentModel;
using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;
using CmlLib.Core.Downloader;

namespace AlphabetUpdate.Client.Updater
{
    public class ZipFileUpdater : IPatchHandler
    {
        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;
        
        public async Task Patch(PatchContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}