using System;
using System.Threading.Tasks;
using AlphabetUpdate.Client.ProcessInteractor;

namespace AuthYouClient.ProcessInteractor
{
    public class AuthYouProcessInteractor : IProcessInteractor
    {
        public AuthYouProcessInteractor(AuthYouClientCore _core)
        {
            this.core = _core;
        }

        public AuthYouProcessInteractor(AuthYouClientSettings settings)
        {
            this.core = new AuthYouClientCore(settings);
        }

        private readonly AuthYouClientCore core;
        
        public void OnProcessStarted()
        {
            Task.Run(() => ready().GetAwaiter().GetResult());
        }

        public void OnProcessExited()
        {
            
        }

        public async void OnProcessOutput(string msg)
        {
            if (msg.Contains("Connecting to") && !msg.Contains("[CHAT]"))
            {
                await connect();
            }
        }
        
        private async Task ready()
        {
            try
            {
                await Task.Delay(5000);
                await core.Ready();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task connect()
        {
            try
            {
                await core.ConnectServer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}