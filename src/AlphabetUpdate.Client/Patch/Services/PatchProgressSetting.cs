using AlphabetUpdate.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Services
{
    public class PatchProgressSetting
    {
        public IProgress<FileChangedEventArg>? FileProgress { get; set; }
        public IProgress<ProgressChangedEventArgs>? DataProgress { get; set; }
        public event EventHandler<string>? Message;

        public void OnMessage(object? sender, string message)
        {
            Message?.Invoke(sender, message);
        }
    }
}
