namespace AlphabetUpdate.Client.ProcessInteractor
{
    public interface IProcessInteractor
    {
        void OnProcessStarted();

        void OnProcessExited();

        void OnProcessOutput(string msg);
    }
}