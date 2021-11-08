using System;
using System.Diagnostics;

namespace AlphabetUpdate.Client.ProcessManage
{
    public abstract class ProcessInteractor
    {
        public ProcessStatus Status { get; private set; } 
        
        public abstract void OnProcessStarted();

        public abstract void OnProcessExited();

        public abstract void OnProcessOutput(string msg);

        public event EventHandler<Exception?>? KillRequest;
        
        protected void Kill()
        {
            KillRequest?.Invoke(this, null);
        }

        protected void Kill(Exception ex)
        {
            KillRequest?.Invoke(this, ex);
        }
    }
}