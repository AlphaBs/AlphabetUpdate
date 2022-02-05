using AlphabetUpdate.Client;
using AlphabetUpdate.Client.Patch.Handler;
using AlphabetUpdate.Client.Patch.Service;
using AlphabetUpdate.Client.Patch.Updater;
using AlphabetUpdate.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddScoped<IPatchHandler, AlphabetFileUpdater>();
        services.AddScoped<IPatchHandler, FileFilterHandler>();

        services.AddScoped<IFileEnabler, FileExtensionEnabler>();
        services.AddScoped<IFileTagService, FileTagService>();
        services.AddScoped<IWhitelistFileService, WhitelistFileService>();
    })
    .Build();


var builder = new LauncherCoreBuilder("path");
var metadata = new LauncherMetadata
{

};

builder.PatchProcess.AddAlphabetFileUpdater(metadata, new AlphabetFileUpdateSetting
{
    
});

builder.PatchProcess.AddFileExtensionEnabler();
builder.PatchProcess.AddFileTagService();
builder.PatchProcess.AddWhitelistFileService(new WhitelistFileSetting
{

});

var launcher = builder.Build();
await launcher.Patch();
