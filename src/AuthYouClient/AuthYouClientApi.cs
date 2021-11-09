using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using AuthYouClient.Models;
using Newtonsoft.Json;

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
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await httpClient.PostAsync($"{Host}/{path}", content);

            var resStr = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resStr);
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
