using System.ComponentModel;
using System.Threading.Tasks;
using CmlLib.Core.Downloader;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class ZipFileUpdater : IPatchHandler
    {
        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;

        private readonly ZipFileUpdaterOptions options;

        public ZipFileUpdater(ZipFileUpdaterOptions opts)
        {
            this.options = opts;
        }

        public async Task Patch(PatchContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}