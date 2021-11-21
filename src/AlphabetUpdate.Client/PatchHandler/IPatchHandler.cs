using AlphabetUpdate.Common.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.PatchHandler
{
    public interface IPatchHandler
    {
        event FileChangedEventHandler? FileChanged;
        event ProgressChangedEventHandler? ProgressChanged;
        Task Patch(PatchContext context);
    }
}