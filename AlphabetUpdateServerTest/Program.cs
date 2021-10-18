using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

            var rawPassword = "`";
            var aesKey = "`";
            var aesIv = "`";
            
            var aes = new AesWrapper(aesKey, aesIv);
            var ms = new MemoryStream();
            await aes.AesEncrypt(new LoginModel
            {
                Name = "devTest",
                Host = "127.0.0.1",
                Password = rawPassword
            }, ms);

            var http = new HttpClient();
            var reqStr = Convert.ToBase64String(ms.ToArray());
            Console.WriteLine(reqStr);
            var reqContent = new StringContent(reqStr);
            reqContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
            var res = await http.PostAsync("https://localhost:5001/v1/Auth/login", reqContent);

            var content = await res.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
    }
}
