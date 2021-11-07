using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;
using AlphabetUpdate.Client.PatchProcess;
using AlphabetUpdate.Client.ProcessInteractor;
using AlphabetUpdate.Common.Models;
using CmlLib.Core;
using CmlLib.Core.Downloader;
using log4net;

namespace AlphabetUpdate.Client
{
    public class LauncherCore
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(LauncherCore));
        
        public bool LogOutput { get; set; }
        public bool LogOutputDebug { get; set; }
        public IProcessInteractor[]? ProcessInteractors { get; set; }
        public event EventHandler<string>? StatusChanged;
        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;
        public event EventHandler? Exited;

        private readonly MinecraftPath minecraftPath;
        
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

        public async Task<Process> Launch(string versionName, MLaunchOption option)
        {
            var launcher = new CMLauncher(minecraftPath);
            launcher.FileChanged += PatcherOnFileChanged;
            launcher.ProgressChanged += PatcherOnProgressChanged;

            var process = await launcher.CreateProcessAsync(versionName, option);
            startProcess(process);
            return process;
        }

        public Task<Process> LaunchFromMetadata(LauncherMetadata metadata, 
            bool useVanilla = false, bool useDirectConnect = true)
        {
            var option = new MLaunchOption();
            
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
                         option.ServerIp = ipspl[0];
                        option.ServerPort = int.Parse(ipspl[1]);
                    }
                    else
                        option.ServerIp = ipspl[0];
                }
            }

            return Launch(startVersion, option);
        }
        
        private void startProcess(Process process)
        {
            log.Info("Setting Process");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += Process_OutputDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Exited += Process_Exited;

            log.Info("Start Process");
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            
            processAction(p => p.OnProcessStarted());
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            log.Info("Process Exited");
            processAction(p => p.OnProcessExited());
            
            Exited?.Invoke(this, EventArgs.Empty);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;
            OnProcessOutput(e.Data);
        }

        void OnProcessOutput(string msg)
        {
            if (LogOutput)
                log.Info(msg);
            if (LogOutputDebug)
                Debug.WriteLine(msg);
            
            processAction(p => p.OnProcessOutput(msg));
        }

        private void processAction(Action<IProcessInteractor> work)
        {
            if (ProcessInteractors != null)
            {
                foreach (var interactor in ProcessInteractors)
                {
                    work(interactor);
                }
            }
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