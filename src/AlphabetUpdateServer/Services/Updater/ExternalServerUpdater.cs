using AlphabetUpdate.Common.Models;
using AlphabetUpdateServer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AlphabetUpdateServer.Services.Updater
{
    public class ExternalServerUpdater : IUpdateService
    {
        private readonly ILogger<ExternalServerUpdater> logger;
        private readonly UpdateFileOptions updateFileOptions;
        private readonly IHttpClientFactory httpClientFactory;

        public ExternalServerUpdater(
            ILogger<ExternalServerUpdater> log,
            IOptions<UpdateFileOptions> opts,
            IHttpClientFactory httpClient)
        {
            this.logger = log;
            this.updateFileOptions = opts.Value;
            this.httpClientFactory = httpClient;
        }

        public async Task<UpdateFileCollection> Update(UpdateFileCollection updateFiles)
        {
            if (updateFileOptions.ExternalServer == null || 
                updateFileOptions.ExternalServer.Length == 0)
                return updateFiles;

            var client = httpClientFactory.CreateClient();
            await Task.WhenAll(updateFileOptions.ExternalServer
                .Select(server => requestPost(client, server, updateFiles)));

            return updateFiles;
        }

        private async Task requestPost(HttpClient client, ExternalServerOptions server, UpdateFileCollection files)
        {
            string responseContent = "";
            try
            {
                logger.LogInformation("Request external server: {0}", server.Url);
                responseContent = "";

                HttpContent requestContent;
                if (string.IsNullOrEmpty(server.Content))
                    requestContent = JsonContent.Create(files);
                else
                    requestContent = new StringContent(server.Content);

                var req = new HttpRequestMessage
                {
                    Method = new HttpMethod(server.Method),
                    RequestUri = new System.Uri(server.Url),
                    Content = requestContent
                };

                if (server.AuthorizationScheme != null && server.AuthorizationParameter != "")
                    req.Headers.Authorization = new AuthenticationHeaderValue(
                        server.AuthorizationScheme, server.AuthorizationParameter);

                var res = await client.SendAsync(req);

                responseContent = await res.Content.ReadAsStringAsync();
                res.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                logger.LogError($"Request Error: {e}\n{responseContent}");
            }
        }
    }
}
