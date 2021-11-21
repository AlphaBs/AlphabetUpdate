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
    public class MinecraftLauncherCore : BaseLauncherCore
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(MinecraftLauncherCore));
        
        public static MinecraftLauncherCoreBuilder CreateBuilder(MinecraftPath path)
            => new MinecraftLauncherCoreBuilder(path);

        private readonly MinecraftPath minecraftPath;
        private readonly MLaunchOption launchOption;

        internal MinecraftLauncherCore(string path, 
            PatchProcess.PatchProcess process,
            ProcessInteractor[]? interactors,
            MinecraftPath minecraft,
            MLaunchOption option)
            : base(path, process, interactors)
        {
            launchOption = option;
            minecraftPath = minecraft;
        }
        
        public async Task<ProcessManager> Launch(string versionName)
        {
            logger.Info("start launch");

            var launcher = new CMLauncher(minecraftPath);
            launcher.FileChanged += Launcher_FileChanged; ;
            launcher.ProgressChanged += Launcher_ProgressChanged;

            var process = await launcher.CreateProcessAsync(versionName, launchOption);
            var manager = new ProcessManager(process, ProcessInteractors);
            manager.Start();
            return manager;
        }

        private void Launcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgressChanged(this, e);
        }

        private void Launcher_FileChanged(DownloadFileChangedEventArgs e)
        {
            OnFileChanged(this, new FileChangedEventArg
            {
                NowFileType = e.FileType.ToString(),
                NowFileName = e.FileName,
                ProgressedFileCount = e.ProgressedFileCount,
                TotalFileCount = e.TotalFileCount
            });
        }
    }
}