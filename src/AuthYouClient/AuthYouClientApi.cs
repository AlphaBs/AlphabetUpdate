using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using AlphabetUpdate.Common.Helpers;
using AuthYouClient.Models;

namespace AuthYouClient
{
    public class AuthYouClientApi
    {
        public AuthYouClientApi(string host, HttpClient http)
        {
            this.httpClient = http;
            this.Host = host;
        }

        private readonly string Host;
        private readonly HttpClient httpClient;

        private async Task<T> Post<T>(string path, object obj)
        {
            var json = JsonSerializer.Serialize(obj, JsonHelper.JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var res = await httpClient.PostAsync($"{Host}/{path}", content);

            var resStr = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(resStr, JsonHelper.JsonOptions);
        }

        public Task<AuthYouTokenResponse> Key(string uuid, HashedFile[] files)
            => Post<AuthYouTokenResponse>("auth/key", new
            {
                uuid = uuid,
                files = files
            });

        public Task<AuthYouResponse> Connect(string uuid, string token)
            => Post<AuthYouResponse>("auth/connect", new
            {
                uuid = uuid,
                token = token
            });
    }
}
