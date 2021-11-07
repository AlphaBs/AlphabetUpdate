using System;
using System.Threading.Tasks;
using AlphabetUpdate.Client.ProcessInteractor;
using AuthYouClient.Models;

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
        
        public async void OnProcessStarted()
        {
            await ready();
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
                // TODO: 10초로 늘리고 10초 지나기 전 connect 하면 ready먼저하고 connect
                // 포지 실행 속도가 생각보다 느림. 5초 안에 파일 추가해도 실행될듯
                await Task.Delay(5000);
                await core.Ready();
            }
            catch (Exception e)
            {
                throw new AuthYouException("Error on ready", e);
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
                throw new AuthYouException("Error on connect", e);
            }
        }
    }
}