using System;
using System.Threading.Tasks;
using AuthYouClient.Models;

namespace AuthYouClient.ProcessInteractor
{
    public class AuthYouProcessInteractor : AlphabetUpdate.Client.ProcessManage.ProcessInteractor
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

        private bool isReady = false;
        private bool startConnect = false;
        
        public override async void OnProcessStarted()
        {
            await ready();
        }

        public override void OnProcessExited()
        {
            
        }

        public override async void OnProcessOutput(string msg)
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
                await Task.Delay(10 * 1000);
                
                if (!startConnect)
                    await core.Ready();
                isReady = true;
            }
            catch (Exception e)
            {
                Kill(new AuthYouException("Error on ready", e));
            }
        }

        private async Task connect()
        {
            startConnect = true;
            
            try
            {
                if (!isReady)
                    await core.Ready();
                await core.ConnectServer();
            }
            catch (Exception e)
            {
                Kill(new AuthYouException("Error on connect", e));
            }
        }
    }
}