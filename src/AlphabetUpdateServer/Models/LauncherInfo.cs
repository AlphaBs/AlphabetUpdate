namespace AlphabetUpdateServer.Models
{
    public class LauncherInfo
    {
        public string? Name { get; init; }

        public string? GameServerIp { get; init; }
        public string? StartVersion { get; init; }
        public string? StartVanillaVersion { get; init; }
        public string? LauncherServer { get; init; }

        public string[]? WhitelistFiles { get; init; }
        public string[]? WhitelistDirs { get; init; }
    }
}
