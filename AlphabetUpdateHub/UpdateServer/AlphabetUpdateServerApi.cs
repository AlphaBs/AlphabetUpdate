using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AlphabetUpdateHub.UpdateServer
{
    public class AlphabetUpdateServerApi
    {
        public static HttpClient DefaultHttpClient = new();
        
        public AlphabetUpdateServerApi(AlphabetUpdateServer server)
            : this(server, DefaultHttpClient)
        {
            
        }
        
        public AlphabetUpdateServerApi(AlphabetUpdateServer server, HttpClient client)
        {
            Server = server;
            httpClient = client;
        }
        
        public string? RequestName { get; set; }
        public string? RequestHost { get; set; }
        public AlphabetUpdateServer Server { get; }
        private readonly HttpClient httpClient;
        private UpdateServerToken? token;

        public async Task<UpdateServerToken> Login()
        {
            if (string.IsNullOrEmpty(Server.AesKey) || string.IsNullOrEmpty(Server.AesIv))
                throw new InvalidOperationException("null aes key, iv");
            
            var aesObjectService = new AesObjectService(Server.AesKey, Server.AesIv);
            
            await using var ms = new MemoryStream();
            await aesObjectService.AesEncrypt(new
            {
                Name = RequestName ?? "Name",
                Host = RequestHost ?? "127.0.0.1",
                Password = Server.Password
            }, ms);

            var content = new StreamContent(ms);
            var res = await httpClient.PostAsync($"{Server.Host}/v1/auth/login", content);
            res.EnsureSuccessStatusCode();
            
            var responseStr = await res.Content.ReadAsStringAsync();
            token = JsonSerializer.Deserialize<UpdateServerToken>(responseStr, JsonHelper.JsonOptions);

            // TODO: ERROR handling, exception
            if (token == null || string.IsNullOrEmpty(token.Token))
                throw new Exception("login failed");

            token.ServerId = Server.ServerId;
            return token;
        }

        private Task<LauncherMetadata?> Get(string path)
            => Get<LauncherMetadata>(path);
        
        private async Task<T?> Get<T>(string path)
        {
            var res = await httpClient.GetAsync($"{Server.Host}/v1/launcher/files");
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<T>();
        }

        private Task<LauncherMetadata?> Post<T>(string path, T obj)
            => Post<T, LauncherMetadata>(path, obj);
        
        private async Task<T2?> Post<T1, T2>(string path, T1 obj)
        {
            if (string.IsNullOrEmpty(token.Token))
                throw new InvalidOperationException("token was empty");

            var content = JsonContent.Create(obj, options: JsonHelper.JsonOptions);
            var res = await httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{Server.Host}/{path}"),
                Headers =
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Content = content
            });
            res.EnsureSuccessStatusCode();

            return await res.Content.ReadFromJsonAsync<T2>();
        }

        public Task<LauncherMetadata?> GetMetadata()
            => Get("v1/launcher");
        
        public Task<LauncherMetadata?> GetLauncherInfo()
            => Get("v1/launcher/info");

        public Task<LauncherMetadata?> UpdateLauncherInfo(LauncherInfo info)
            => Post("v1/launcher/info", info);

        public Task<LauncherMetadata?> GetLauncherFiles()
            => Get("v1/launcher/files");

        public Task<LauncherMetadata?> UpdateLauncherFiles(UpdateFileCollection files)
            => Post("v1/launcher/files", files);

        public Task<UpdateFileCollection?> UpdateInputFiles()
            => Get<UpdateFileCollection>("v1/updateFile");
    }
}