using AlphabetUpdate.Client.PatchProcess;
using AlphabetUpdate.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.PatchHandler
{
    public static class Extensions
    {
        public static PatchProcessBuilder AddAlphabetFileUpdater(this PatchProcessBuilder b,
            LauncherMetadata metadata)
        {
            return AddAlphabetFileUpdater(b, metadata, new AlphabetFileUpdaterOptions());
        }

        public static PatchProcessBuilder AddAlphabetFileUpdater(this PatchProcessBuilder b,
            LauncherMetadata metadata, AlphabetFileUpdaterOptions options)
        {
            options.BaseUrl = metadata.Launcher?.LauncherServer;
            b.SetOptions(o =>
            {
                o.WhitelistDirs = metadata.Launcher?.WhitelistDirs;
                o.WhitelistFiles = metadata.Launcher?.WhitelistFiles;
            });

            if (metadata.Files != null)
                b.AddHandler(new AlphabetFileUpdater(metadata.Files, options));
            return b;
        }

        public static PatchProcessBuilder AddZipFileUpdater(this PatchProcessBuilder b,
            ZipFileUpdaterOptions options)
        {
            b.AddHandler(new ZipFileUpdater(options));
            return b;
        }
    }
}
