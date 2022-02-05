namespace AlphabetUpdate.Client.Patch
{
    public class PatchOptions
    {
        public PatchOptions(string path)
        {
            BasePath = path;
        }

        public string[]? WhitelistDirs { get; set; }
        public string[]? WhitelistFiles { get; set; }
        public string BasePath { get; set; }
    }
}