using System;
using System.Linq;
using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;
using AlphabetUpdate.Client.Updater;
using AlphabetUpdate.Client.UpdateServer;
using AlphabetUpdate.Common.Models;
using CmlLib.Core;

namespace AlphabetUpdate.Client.PatchProcess
{
    public static class AlphabetPatchProcessBuilder
    {
        public static PatchProcessBuilder AddAlphabetFileUpdater(this PatchProcessBuilder b,
            LauncherMetadata metadata)
        {
            return AddAlphabetFileUpdater(b, metadata, new AlphabetFileUpdaterOptions());
        }
        
        public static PatchProcessBuilder AddAlphabetFileUpdater(this PatchProcessBuilder b, 
            LauncherMetadata metadata, AlphabetFileUpdaterOptions options)
        {
            options.BaseUrl = metadata.Launcher.LauncherServer;
            b.SetOptions(o =>
            {
                o.WhitelistDirs = metadata.Launcher.WhitelistDirs;
                o.WhitelistFiles = metadata.Launcher.WhitelistFiles;
            });

            b.AddHandler(new AlphabetFileUpdater(metadata.Files, options));
            return b;
        }
    }
}