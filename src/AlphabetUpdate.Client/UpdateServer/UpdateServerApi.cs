using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdate.Client.UpdateServer
{
    public class UpdateServerApi : IUpdateServerApi
    {
        public UpdateServerApi(string host) : this(host, HttpHelper.HttpClient)
        {
            
        }
        
        public UpdateServerApi(string host, HttpClient http)
        {
            this.httpClient = http;
            this.host = host;
        }

        private readonly HttpClient httpClient;
        private readonly string host;

        private async Task<T> Get<T>(string path)
        {
            var res = await httpClient.GetAsync($"{host}/{path}");
            var resStr = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(resStr, JsonHelper.JsonOptions);
        }

        public Task<LauncherMetadata?> GetLauncherMetadata()
            => Get<LauncherMetadata?>($"v1/launcher");
        
        public Task<LauncherInfo?> GetLauncherInfo()
            => Get<LauncherInfo?>($"v1/launcher/info");

        public Task<UpdateFileCollection?> GetUpdateFileCollection()
            => Get<UpdateFileCollection?>($"v1/launcher/files");
    }
}