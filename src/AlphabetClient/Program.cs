using AlphabetUpdate.Client.UpdateServer;
using AlphabetUpdate.Client.Patch.Core;
using AlphabetUpdate.Client.Patch.Core.Handlers;
using AlphabetUpdate.Client.Patch.Handlers;
using AlphabetUpdate.Client.Patch.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AlphabetUpdate.Common.Models;
using System.ComponentModel;
using AlphabetUpdate.Client.Patch.Core.Services;
using AlphabetUpdate.Common.Helpers;

// build host
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddPatchServices();
    })
    .Build();

// UpdateServer
var u1 = "http://13.124.250.194/updateserver";
var u2 = "http://13.124.250.194/updateserver";
var updateServer = new UpdateServerApi(u2);
var metadata = await updateServer.GetLauncherMetadata();
if (metadata == null)
{
    Console.WriteLine("null metadata");
    return;
}

// PatchProcess
var patchProcess = new PatchProcess();
patchProcess.AddPatchProgressService(new PatchProgressSetting
{
    FileProgress = new Progress<FileChangedEventArg>(args =>
    {
        Console.WriteLine($"[{args.NowFileType}] {args.NowFileName} ({args.ProgressedFileCount} / {args.TotalFileCount})");
    }),
    DataProgress = new Progress<ProgressChangedEventArgs>(args =>
    {
        //Console.WriteLine($"{args.ProgressPercentage}%");
    })
});
patchProcess.AddWhitelistFileService(new WhitelistFileSetting
{
    WhiteFiles = metadata.Launcher?.WhitelistFiles ?? new[] 
    {
        "options.txt"
    },
    WhiteDirs = metadata.Launcher?.WhitelistDirs ?? new[]
    {
        "personal",
        "settings"
    }
});

patchProcess.AddAlphabetFileUpdater(metadata);
patchProcess.AddFileFilter(new FileFilterSetting());

// PatchProcessor
Console.WriteLine("start patch");
var patchProcessor = new PatchProcessor(host.Services);
await patchProcessor.Patch(patchProcess, new PatchOptions
{
    BasePath = "test"
}, null);

Console.WriteLine("done");