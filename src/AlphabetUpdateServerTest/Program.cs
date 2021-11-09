using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Services;
using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Test().GetAwaiter().GetResult();
        }

        private async Task Test()
        {
            //var ss = SecureAesStorage.FromAes(new AesWrapper(
            //    "`", 
            //    "`"));
            //var ssObj = await ss.Load();

            var rawPassword = "87c919dbcabc415192b1a1abb2cb33c8";
            var aesKey = "KI8J25mDMV2f/Gdhcm10erb1/egVPD2ucOkjJxlOtzw=";
            var aesIv = "hl\u002BW6JC7TMWwLbwRzuTTIg==";
            
            var aes = new AesObjectService(aesKey, aesIv);
            var ms = new MemoryStream();
            await aes.AesEncrypt(new LoginModel
            {
                Name = "devTest",
                Host = "127.0.0.1",
                Password = rawPassword
            }, ms);
            var reqStr = Convert.ToBase64String(ms.ToArray());

            var http = new HttpClient();
            Console.WriteLine(reqStr);
            //return;
            var reqContent = new StringContent(reqStr);
            reqContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
            var res = await http.PostAsync("http://localhost:5000/v1/auth/login", reqContent);

            var content = await res.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
    }
}
