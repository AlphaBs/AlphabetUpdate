using AlphabetUpdate.Client.PatchHandler;
using AlphabetUpdate.Client.ProcessManage;
using AlphabetUpdate.Common.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client
{
    public class BaseLauncherCore
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(BaseLauncherCore));

        public bool LogOutput { get; set; }
        public bool LogOutputDebug { get; set; }
        public ProcessInteractor[]? ProcessInteractors { get; private set; }
        public event EventHandler<string>? StatusChanged;
        public event FileChangedEventHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;

        protected readonly string ClientPath;
        protected readonly PatchProcess.PatchProcess PatchProcess;

        public BaseLauncherCore(string path, PatchProcess.PatchProcess patch, ProcessInteractor[]? interactors)
        {
            ClientPath = path;
            PatchProcess = patch;
            ProcessInteractors = interactors;
        }

        public async Task Patch()
        {
            logger.Info("start patch");

            var patchContext = new PatchContext(PatchProcess.Options);
            patchContext.StatusChanged += OnStatusChanged;

            foreach (var patcher in PatchProcess.Patchers)
            {
                patcher.FileChanged += OnFileChanged;
                patcher.ProgressChanged += OnProgressChanged;

                await patcher.Patch(patchContext);

                patcher.FileChanged -= OnFileChanged;
                patcher.ProgressChanged -= OnProgressChanged;
            }
        }

        protected void OnProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        protected void OnFileChanged(object sender, FileChangedEventArg e)
        {
            FileChanged?.Invoke(sender, e);
        }

        protected void OnStatusChanged(object? sender, string message)
        {
            StatusChanged?.Invoke(this, message);
        }
    }
}
