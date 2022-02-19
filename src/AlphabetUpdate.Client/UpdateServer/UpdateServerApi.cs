using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdate.Client.UpdateServer
{
    public class UpdateServerApi : IUpdateServerApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _host;

        public UpdateServerApi(string host) : this(host, HttpHelper.HttpClient)
        {
            
        }
        
        public UpdateServerApi(string host, HttpClient http)
        {
            this._httpClient = http;
            this._host = host;
        }

        private async Task<T?> Get<T>(string path)
        {
            var res = await _httpClient.GetAsync($"{_host}/{path}");
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