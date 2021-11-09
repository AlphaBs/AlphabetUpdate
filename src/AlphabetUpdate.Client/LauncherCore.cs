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
        
        public bool LogOutput { get; set; }
        public bool LogOutputDebug { get; set; }
        public ProcessInteractor[]? ProcessInteractors { get; set; }
        public event EventHandler<string>? StatusChanged;
        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;
        public event EventHandler? Exited;

        private readonly MinecraftPath minecraftPath;
        private MLaunchOption launchOption = new MLaunchOption();
        private Action<MLaunchOption>? launchOptionSetter;

        public LauncherCore(MinecraftPath path)
        {
            minecraftPath = path;
        }
        
        public async Task Patch(IPatchProcessBuilder builder)
        {
            var patchProcess = await builder.Build();
            await Patch(patchProcess);
        }

        public async Task Patch(Func<Task<PatchProcess.PatchProcess>> processGenerator)
        {
            var patchProcess = await processGenerator();
            await Patch(patchProcess);
        }

        public async Task Patch(PatchProcess.PatchProcess patchProcess)
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

        public Action<MLaunchOption> SetLaunchOption(Action<MLaunchOption> fn)
            => launchOptionSetter = fn;
        
        public async Task<ProcessManager> Launch(string versionName)
        {
            logger.Info("start launch");
            launchOptionSetter?.Invoke(launchOption);
            
            var launcher = new CMLauncher(minecraftPath);
            launcher.FileChanged += PatcherOnFileChanged;
            launcher.ProgressChanged += PatcherOnProgressChanged;

            var process = await launcher.CreateProcessAsync(versionName, launchOption);
            var manager = new ProcessManager(process, ProcessInteractors);
            manager.Start();
            return manager;
        }

        public Task<ProcessManager> LaunchFromMetadata(LauncherMetadata metadata, 
            bool useVanilla = false, bool useDirectConnect = true)
        {
            var startVersion = metadata.Launcher.StartVersion;
            if (useVanilla && !string.IsNullOrEmpty(metadata.Launcher.StartVanillaVersion))
                startVersion = metadata.Launcher.StartVanillaVersion;
            
            if (useDirectConnect)
            {
                var ipspl = metadata.Launcher.GameServerIp.Split(':');
                if (ipspl.Length != 0)
                {
                    if (ipspl.Length == 2)
                    {
                        launchOption.ServerIp = ipspl[0];
                        launchOption.ServerPort = int.Parse(ipspl[1]);
                    }
                    else
                        launchOption.ServerIp = ipspl[0];
                }
            }

            return Launch(startVersion);
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