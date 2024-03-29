﻿using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.ProcessManage
{
    public class ProcessManager
    {
        private readonly ILogger<ProcessManager> _logger;

        public event EventHandler<ProcessResult>? Exited;
        public bool LogOutput { get; set; }
        public bool LogOutputDebug { get; set; }
        public ProcessStatus Status { get; private set; }
        public  Process Process { get; }

        public Exception? ProcessException { get; private set; }
        private string? lastOutput;
        
        public ProcessManager(Process proc, ProcessInteractor[]? interactors, ILogger<ProcessManager> logger)
        {
            Process = proc;
            processInteractors = interactors;
            _logger = logger;
        }

        private readonly ProcessInteractor[]? processInteractors;

        public void Start()
        {
            _logger.LogInformation("Setting Process");
            ProcessException = null;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.ErrorDataReceived += Process_OutputDataReceived;
            Process.OutputDataReceived += Process_OutputDataReceived;
            Process.Exited += Process_Exited;

            _logger.LogInformation("Start Process");
            Process.Start();
            
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }

        public void Interact()
        {
            Process.EnableRaisingEvents = true;
            if (processInteractors != null)
            {
                foreach (var inter in processInteractors)
                {
                    inter.Enabled = true;
                }
            }

            processAction(p =>
            {
                p.KillRequest += InteractorOnKill;
                p.OnProcessStarted();
            });
        }

        public void Kill()
        {
            if (Status == ProcessStatus.Running)
            {
                Status = ProcessStatus.Killing;
                Process.Kill();
            }
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
                _logger.LogInformation(msg);
            if (LogOutputDebug)
                Debug.WriteLine(msg);
            lastOutput = msg;
            
            processAction(p => p.OnProcessOutput(msg));
        }

        private void InteractorOnKill(object sender, Exception? e)
        {
            if (Status != ProcessStatus.Running)
                return;

            ProcessException = e;
            Status = ProcessStatus.Killing;
            Process.Kill();
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            _logger.LogInformation("Process Exited");

            ProcessResult result = new ProcessResult(Process.ExitCode)
            {
                Message = lastOutput,
                Exception = ProcessException
            };

            processAction(p => p.OnProcessExited());
            Status = ProcessStatus.Killed;
            
            Exited?.Invoke(this, result);
        }
        
        private void processAction(Action<ProcessInteractor> work)
        {
            if (processInteractors == null)
                return;
            
            foreach (var inter in processInteractors)
            {
                if (inter.Enabled)
                    work(inter);
            }
        }
    }
}