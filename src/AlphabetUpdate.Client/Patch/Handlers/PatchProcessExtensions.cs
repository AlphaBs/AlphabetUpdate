using AlphabetUpdate.Client.Patch.Core;
using AlphabetUpdate.Client.Patch.Handlers;
using AlphabetUpdate.Client.Patch.Services;
using AlphabetUpdate.Common.Models;

namespace AlphabetUpdate.Client.Patch.Handlers
{
    public static class PatchProcessExtensions
    {
        // PatchHandlers

        public static void AddAlphabetFileUpdater(this PatchProcess b,
            LauncherMetadata metadata)
        {
            AddAlphabetFileUpdater(b, metadata, new AlphabetFileUpdateSetting());
        }

        public static void AddAlphabetFileUpdater(this PatchProcess b,
            LauncherMetadata metadata, AlphabetFileUpdateSetting settings)
        {
            settings.BaseUrl = metadata.Launcher?.LauncherServer;
            
            // add whitelist

            if (metadata.Files != null)
                b.AddPatchHandler<AlphabetFileUpdater, AlphabetFileUpdateSetting>(settings);
        }

        public static void AddZipFileUpdater(this PatchProcess b,
            ZipFileUpdateSetting settings)
        {
            b.AddPatchHandler<ZipFileUpdater, ZipFileUpdateSetting>(settings);
        }

        public static void AddFileFilter(this PatchProcess b,
            FileFilterSetting settings)
        {
            b.AddPatchHandler<FileFilterHandler ,FileFilterSetting>(settings);
        }

        // PatchServices

        public static void AddWhitelistFileService(this PatchProcess b,
            WhitelistFileSetting setting)
        {
            b.AddPatchService<WhitelistFileService, WhitelistFileSetting>(setting);
        }

        public static void AddPatchProgressService(this PatchProcess b,
            PatchProgressSetting setting)
        {
            b.AddPatchService<PatchProgressService, PatchProgressSetting>(setting);
        }

        public static void AddFileTagService(this PatchProcess b)
        {
            b.AddPatchService(new FileTagService());
        }

        public static void AddFileExtensionEnabler(this PatchProcess b)
        {
            b.AddPatchService(new FileExtensionEnabler());
        }
    }
}
