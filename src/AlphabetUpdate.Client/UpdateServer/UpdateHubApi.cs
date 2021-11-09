using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdate.Client.UpdateServer
{
    public class UpdateHubApi : IUpdateServerApi
    {
        public UpdateHubApi(string host, string serverId)
            : this(host, serverId, HttpHelper.HttpClient)
        {
            
        }
        
        public UpdateHubApi(string host, string serverId, HttpClient http)
        {
            this.httpClient = http;
            this.host = host;
            this.serverId = serverId;
        }

        private readonly HttpClient httpClient;
        private readonly string host;
        private readonly string serverId;

        private async Task<T> Get<T>(string path)
        {
            var res = await httpClient.GetAsync($"{host}/{path}");
            var resStr = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(resStr, JsonHelper.JsonOptions);
        }

        public Task<LauncherMetadata?> GetLauncherMetadata()
            => Get<LauncherMetadata?>($"servers/{serverId}");
        
        public Task<LauncherInfo?> GetLauncherInfo()
            => Get<LauncherInfo?>($"servers/{serverId}/info");

        public Task<UpdateFileCollection?> GetUpdateFileCollection()
            => Get<UpdateFileCollection?>($"servers/{serverId}/files");
    }
}