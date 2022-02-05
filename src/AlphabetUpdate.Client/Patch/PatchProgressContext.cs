using AlphabetUpdate.Common.Models;
using System;
using System.ComponentModel;

namespace AlphabetUpdate.Client.Patch
{
    // 진행률 표시
    public class PatchProgressContext : PatchProgressService
    {
        private readonly IProgress<FileChangedEventArg>? _fileProgress;
        private readonly IProgress<ProgressChangedEventArgs>? _progressProgress;

        public PatchProgressContext(
            IProgress<FileChangedEventArg>? fileProgress,
            IProgress<ProgressChangedEventArgs>? progressProgress)
        {
            _fileProgress = fileProgress;
            _progressProgress = progressProgress;
        }

        public event EventHandler<string>? Message;

        public void OnFileChanged(object? sender, FileChangedEventArg args)
        {
            _fileProgress?.Report(args);
        }

        public void OnProgressChanged(object? sender, ProgressChangedEventArgs args)
        {
            _progressProgress?.Report(args);
        }

        public void OnMessage(object? sender, string message)
        {
            Message?.Invoke(sender, message);
        }
    }
}
