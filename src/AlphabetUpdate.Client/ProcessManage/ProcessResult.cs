using System;

namespace AlphabetUpdate.Client.ProcessManage
{
    public class ProcessResult
    {
        public static readonly ProcessResult Normal = new ProcessResult(0);

        public ProcessResult(int exitCode)
        {
            this.ExitCode = exitCode;
        }

        public int ExitCode { get; set; }
        public Exception? Exception { get; set; }
        public string? Message { get; set; }
    }
}