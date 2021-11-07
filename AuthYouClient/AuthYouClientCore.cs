using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AuthYouClient.Models;

namespace AuthYouClient
{
    public class AuthYouClientCore
    {
        public AuthYouClientCore(AuthYouClientSettings settings)
        {
            this.uuid = settings.Uuid;
            this.clientApi = settings.ClientApi;
            this.tokenEncryptor = settings.TokenEncryptor;
            this.basePath = settings.BasePath;
            this.targetDirs = settings.TargetDirs;
        }

        private readonly AuthYouClientApi clientApi;
        private readonly string uuid;
        private readonly ITokenEncryptor tokenEncryptor;

        private readonly string basePath;
        private readonly string[] targetDirs;

        private AuthYouTokenResponse? token;

        private HashedFile[] GetProtectedFiles()
        {
            var lst = new List<HashedFile>();
            foreach (var path in targetDirs)
            { 
                GetHashedFiles(Path.Combine(basePath, path), lst);
            }

            return lst.ToArray();
        }

        private void GetHashedFiles(string dirPath, List<HashedFile> lst)
        { 
            var dir = new DirectoryInfo(dirPath);
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fullPath = file.FullName;
                
                lst.Add(new HashedFile
                {
                    Path = fullPath
                        .Replace(basePath, "")
                        .Replace("\\", "/")
                        .Trim('/'),
                    Hash = CryptoHelper.ToHexString(CryptoHelper.HashMd5(fullPath))
                });
            }
        }

        public async Task Ready()
        {
            var res = await clientApi.Key(uuid, GetProtectedFiles());
            if (!res.Result)
                throw new AuthYouException(res);
            this.token = res;
        }

        public async Task ConnectServer()
        {
            if (token == null || !token.Result)
                throw new AuthYouException("Not ready");

            var newToken = tokenEncryptor.Encrypt(token.Token);
            var res = await clientApi.Connect(uuid, newToken);

            if (!res.Result)
                throw new AuthYouException(res);
        }
    }
}
