using AlphabetUpdate.Common.Models;
using System.ComponentModel;

namespace AlphabetUpdate.Client.Patch
{
    public interface IPatchProgressContext
    {
        void OnFileChanged(object? sender, FileChangedEventArg args);
        void OnProgressChanged(object? sender, ProgressChangedEventArgs args);
        void OnMessage(object? sender, string message);
    }
}
