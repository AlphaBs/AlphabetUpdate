using System.ComponentModel;
using System.Threading.Tasks;
using CmlLib.Core.Downloader;

namespace AlphabetUpdate.Client.PatchHandler
{
    public interface IPatchHandler
    {
        event DownloadFileChangedHandler? FileChanged;
        event ProgressChangedEventHandler? ProgressChanged;
        Task Patch(PatchContext context);
    }
}