using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.UpdateServer
{
    public class StaticUpdateServerApi : IUpdateServerApi
    {
        private readonly string _host;
        private readonly HttpClient _httpClient;

        public StaticUpdateServerApi(string host) : this(host, HttpHelper.HttpClient)
        {

        }

        public StaticUpdateServerApi(string host, HttpClient http)
        {
            _host = host;
            _httpClient = http;
        }

        public async Task<LauncherMetadata?> GetLauncherMetadata()
        {
            var res = await _httpClient.GetAsync(_host);
            var resStr = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LauncherMetadata>(resStr, JsonHelper.JsonOptions);
        }

        public async Task<LauncherInfo?> GetLauncherInfo()
        {
            var metadata = await GetLauncherMetadata();
            return metadata?.Launcher;
        }

        public async Task<UpdateFileCollection?> GetUpdateFileCollection()
        {
            var metadata = await GetLauncherMetadata();
            return metadata?.Files;
        }
    }
}
