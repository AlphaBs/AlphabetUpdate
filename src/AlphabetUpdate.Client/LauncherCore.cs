using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;
using AlphabetUpdate.Client.PatchProcess;
using AlphabetUpdate.Client.ProcessManage;
using AlphabetUpdate.Common.Models;
using CmlLib.Core;
using CmlLib.Core.Downloader;
using log4net;

namespace AlphabetUpdate.Client
{
    public class LauncherCore
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(LauncherCore));
        
        public static LauncherCoreBuilder CreateBuilder(MinecraftPath path)
            => new LauncherCoreBuilder(path);

        public bool LogOutput { get; set; }
        public bool LogOutputDebug { get; set; }
        public ProcessInteractor[]? ProcessInteractors { get; set; }
        public event EventHandler<string>? StatusChanged;
        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;

        private readonly MinecraftPath minecraftPath;
        private readonly PatchProcess.PatchProcess patchProcess;
        private readonly MLaunchOption launchOption;

        public LauncherCore(MinecraftPath path, 
            PatchProcess.PatchProcess process,
            ProcessInteractor[]? interactors,
            MLaunchOption option)
        {
            minecraftPath = path;
            patchProcess = process;
            ProcessInteractors = interactors;
            launchOption = option;
        }

        public async Task Patch()
        {
            logger.Info("start patch");
            
            if (patchProcess.Options.MinecraftPath == null)
                patchProcess.Options.MinecraftPath = minecraftPath;
            
            var patchContext = new PatchContext(patchProcess.Options);
            patchContext.StatusChanged += OnStatusChanged;
            
            foreach (var patcher in patchProcess.Patchers)
            {
                patcher.FileChanged += PatcherOnFileChanged;
                patcher.ProgressChanged += PatcherOnProgressChanged;
                
                await patcher.Patch(patchContext);
                
                patcher.FileChanged -= PatcherOnFileChanged;
                patcher.ProgressChanged -= PatcherOnProgressChanged;
            }
        }
        
        public async Task<ProcessManager> Launch(string versionName)
        {
            logger.Info("start launch");

            var launcher = new CMLauncher(minecraftPath);
            launcher.FileChanged += PatcherOnFileChanged;
            launcher.ProgressChanged += PatcherOnProgressChanged;

            var process = await launcher.CreateProcessAsync(versionName, launchOption);
            var manager = new ProcessManager(process, ProcessInteractors);
            manager.Start();
            return manager;
        }

        private void PatcherOnProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        private void PatcherOnFileChanged(DownloadFileChangedEventArgs e)
        {
            FileChanged?.Invoke(e);
        }

        private void OnStatusChanged(object? sender, string message)
        {
            StatusChanged?.Invoke(this, message);
        }
    }
}