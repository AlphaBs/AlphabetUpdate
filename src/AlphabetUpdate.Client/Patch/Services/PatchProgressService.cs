using AlphabetUpdate.Client.Patch.Core.Services;
using AlphabetUpdate.Common.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Services
{
    // 진행률 표시
    public class PatchProgressService : PatchServiceBase<PatchProgressSetting>, IPatchProgressService
    {
        public override Task Initialize()
        {
            return Task.CompletedTask;
        }

        public void OnFileChanged(object? sender, FileChangedEventArg args)
        {
            Setting?.FileProgress?.Report(args);
        }

        public void OnProgressChanged(object? sender, ProgressChangedEventArgs args)
        {
            Setting?.DataProgress?.Report(args);
        }

        public void OnMessage(object? sender, string message)
        {
            Setting?.OnMessage(sender, message);
        }
    }
}
