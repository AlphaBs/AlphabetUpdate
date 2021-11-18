using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Downloader;
using CmlLib.Utils;
using log4net;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class OptionsPatchHandler : IPatchHandler
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(OptionsPatchHandler));
        
        public OptionsPatchHandler(GameOptions opts)
        {
            this.gameOptions = opts;
        }

        private readonly GameOptions gameOptions;

        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;

        public Task Patch(PatchContext context)
        {
            var optionPath = Path.Combine(context.MinecraftPath.BasePath, "options.txt");

            GameOptionsFile optionFile;
            if (File.Exists(optionPath))
                optionFile = GameOptionsFile.ReadFile(optionPath);
            else
                optionFile = new GameOptionsFile();

            if (gameOptions.ResourcePacks != null)
                addResourcePacks(optionFile, gameOptions.ResourcePacks);

            optionFile.Save();
            return Task.CompletedTask;
        }

        private void addResourcePacks(GameOptionsFile options, string[] names)
        {
            var rawValue = string.Join(",", names.Select(name => $"\"{Path.GetFileName(name)}\""));
            options?.SetRawValue("resourcePacks", $"[{rawValue}]");
        }
    }
}