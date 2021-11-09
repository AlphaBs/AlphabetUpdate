using System;
using System.Threading.Tasks;
using AuthYouClient.Models;
using log4net;

namespace AuthYouClient.ProcessInteractor
{
    public class AuthYouProcessInteractor : AlphabetUpdate.Client.ProcessManage.ProcessInteractor
    {
        private static readonly ILog log = LogManager.GetLogger("AuthYou");
        
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
        private bool isRunning = false;
        
        public override async void OnProcessStarted()
        {
            if (isRunning)
                return;

            log.Info("Start");
            
            isRunning = true;
            while (isRunning)
            {
                await ready();
                await Task.Delay(1 * 60 * 1000);
            }
        }

        public override void OnProcessExited()
        {
            isRunning = false;
            log.Info("Exited");
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
                log.Info("Connect");
                if (!isReady)
                    await core.Ready();
                await core.ConnectServer();

                startConnect = false;
            }
            catch (Exception e)
            {
                Kill(new AuthYouException("Error on connect", e));
            }
        }
    }
}