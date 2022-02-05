using AlphabetUpdate.Client.Patch;
using AlphabetUpdate.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Updater
{
    public static class Extensions
    {
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

        public static void AddWhitelistFileService(this PatchProcess b,
            WhitelistFileSetting setting)
        {
            b.AddPatchService<WhitelistFileService, WhitelistFileSetting>(setting);
        }

        public static void AddFileTagService(this PatchProcess b)
        {
            //b.AddPatchService<FileTagService>(new File);
        }

        public static void AddFileExtensionEnabler(this PatchProcess b)
        {
            //b.AddPatchService<FileExtensionEnabler>(new FileExtensionEnabler());
        }

        public static void AddZipFileUpdater(this PatchProcess b,
            ZipFileUpdateSetting settings)
        {
            b.AddPatchHandler<ZipFileUpdater, ZipFileUpdateSetting>(settings);
        }
    }
}
