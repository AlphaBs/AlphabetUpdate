using CmlLib.Core;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class PatchOptions
    {
        public string[]? WhitelistDirs { get; set; }
        public string[]? WhitelistFiles { get; set; }
        public string[]? Tags { get; set; }
        public MinecraftPath? MinecraftPath { get; set; }
    }
}