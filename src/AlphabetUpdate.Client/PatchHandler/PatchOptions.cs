namespace AlphabetUpdate.Client.PatchHandler
{
    public class PatchOptions
    {
        public PatchOptions(string path)
        {
            ClientPath = path;
        }

        public string[]? WhitelistDirs { get; set; }
        public string[]? WhitelistFiles { get; set; }
        public string[]? Tags { get; set; }
        public string ClientPath { get; set; }
    }
}