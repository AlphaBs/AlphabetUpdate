using Microsoft.Extensions.DependencyInjection;

namespace AlphabetUpdate.Client.Patch.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPatchServices(this IServiceCollection collection)
        {
            collection.AddScoped<IFileEnabler, FileExtensionEnabler>();
            collection.AddScoped<IFileTagService, FileTagService>();
            collection.AddScoped<IPatchProgressService, PatchProgressService>();
            collection.AddScoped<IWhitelistFileService, WhitelistFileService>();
        }
    }
}
