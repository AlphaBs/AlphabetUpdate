using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Client;
using AlphabetUpdate.Client.PatchHandler;
using AlphabetUpdate.Client.PatchProcess;
using AlphabetUpdate.Client.ProcessManage;
using AlphabetUpdate.Client.UpdateServer;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Models;
using AuthYouClient;
using AuthYouClient.ProcessInteractor;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;

namespace AlphabetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.start().GetAwaiter().GetResult();
            //p.testAuthYou().GetAwaiter().GetResult();
        }

        private async Task testAuthYou()
        {
            var auth = new AuthYouClientCore(new AuthYouClientSettings
            {
                ClientApi = new AuthYouClientApi("https://alphabeta.pw/alphabet/authyou/m10", HttpHelper.HttpClient),
                Uuid = "asdf",
                BasePath = Path.GetFullPath("testgd"),
                TargetDirs = new[] { "mods" }
            });

            await auth.Ready();
            await auth.ConnectServer();
        }

        private async Task start()
        {
            var authYouHost = "https://alphabeta.pw/alphabet/authyou/m10";
            var updateServerHost = "http://3.34.192.206/launcher/files-al.json";
            //var authYouHost = "https://alphabeta.pw/alphabet/authyou/m11";
            //var updateServerHost = "http://15.165.237.254/launcher/files.json";
            var session = MSession.GetOfflineSession("tester123");
            session.UUID = "4e7e3bea-4e3a-3db3-8ca0-af6a0f6391f8";

            var vanilla = new MinecraftPath();
            var path = new MinecraftPath("testgd");
            path.Assets = vanilla.Assets;
            path.Runtime = vanilla.Runtime;

            //var api = new UpdateServerApi(updateServerHost, HttpHelper.HttpClient);
            //var metadata = await api.GetLauncherMetadata();
            var res = await HttpHelper.HttpClient.GetAsync(updateServerHost);
            var metadata = await res.Content.ReadFromJsonAsync<LauncherMetadata>(JsonHelper.JsonOptions);

            var builder = MinecraftLauncherCore.CreateBuilder(path);
            builder.PatchProcess.AddAlphabetFileUpdater(metadata, new AlphabetFileUpdaterOptions
            {
                LastUpdateFilePath = "last",
                AlwaysUpdates = new[] { "mods" }
            });
            //builder.PatchProcess.AddZipFileUpdater(new ZipFileUpdaterOptions(DateTime.Parse("2021-11-19"))
            //{
            //    LastUpdateFilePath = "last_zip",
            //    ZipStream = File.OpenRead("test.zip")
            //});
            builder.AddAuthYouInteractor(new AuthYouClientSettings
            {
                ClientApi = new AuthYouClientApi(authYouHost, HttpHelper.HttpClient),
                Uuid = session.UUID,
                BasePath = path.BasePath,
                TargetDirs = new[] { "mods" },
            });
            //builder.UseDirectServerConnection(metadata.Launcher.GameServerIp);
            builder.UseDirectServerConnection("127.0.0.1");
            builder.AddLaunchOptionAction((MLaunchOption option) =>
            {
                option.MaximumRamMb = 4096;
            });

            var core = builder.BuildMinecraftLauncher();
            core.FileChanged += CoreOnFileChanged;
            core.ProgressChanged += CoreOnProgressChanged;

            await core.Patch();
            //var process = await core.Launch(metadata.Launcher.StartVersion);
            var process = await core.Launch(metadata.Launcher.StartVersion);
            process.Exited += ProcessOnExited;
            process.Interact();
            
            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        private void ProcessOnExited(object sender, ProcessResult e)
        {
            if (e.Exception != null)
                Console.WriteLine(e.Exception);
            
            Console.WriteLine(e.ExitCode);
            Console.WriteLine(e.Message);
        }

        private void CoreOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage);
        }

        private void CoreOnFileChanged(object sender, FileChangedEventArg e)
        {
            Console.WriteLine($"{e.NowFileName} {e.ProgressedFileCount} {e.TotalFileCount}");
        }
    }
}