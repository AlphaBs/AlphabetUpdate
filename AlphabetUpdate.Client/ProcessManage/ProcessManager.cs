using System;
using System.Diagnostics;
using System.Threading.Tasks;
using log4net;

namespace AlphabetUpdate.Client.ProcessManage
{
    public class ProcessManager
    {
        private ILog logger = LogManager.GetLogger(nameof(ProcessManager));

        public event EventHandler<ProcessResult>? Exited;
        public bool LogOutput { get; set; }
        public bool LogOutputDebug { get; set; }
        public ProcessStatus Status { get; private set; }
        public  Process Process { get; }

        private Exception? processException;
        private string? lastOutput;
        
        public ProcessManager(Process proc, ProcessInteractor[] interactors)
        {
            Process = proc;
            processInteractors = interactors;
        }

        private readonly ProcessInteractor[] processInteractors;

        public void Start()
        {
            logger.Info("Setting Process");
            processException = null;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.EnableRaisingEvents = true;
            Process.ErrorDataReceived += Process_OutputDataReceived;
            Process.OutputDataReceived += Process_OutputDataReceived;
            Process.Exited += Process_Exited;

            logger.Info("Start Process");
            Process.Start();
        }

        public void Interact()
        {
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();

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
                logger.Info(msg);
            if (LogOutputDebug)
                Debug.WriteLine(msg);
            lastOutput = msg;
            
            processAction(p => p.OnProcessOutput(msg));
        }

        private void InteractorOnKill(object sender, Exception? e)
        {
            if (Status != ProcessStatus.Running)
                return;

            processException = e;
            Status = ProcessStatus.Killing;
            Process.Kill();
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            logger.Info("Process Exited");

            ProcessResult result = new ProcessResult(Process.ExitCode)
            {
                Message = lastOutput,
                Exception = processException
            };

            processAction(p => p.OnProcessExited());
            Status = ProcessStatus.Killed;
            
            Exited?.Invoke(this, result);
        }
        
        private void processAction(Action<ProcessInteractor> work)
        {
            foreach (var inter in processInteractors)
            {
                work(inter);
            }
        }
    }
}