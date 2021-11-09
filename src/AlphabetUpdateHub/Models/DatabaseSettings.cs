namespace AlphabetUpdateHub.Models
{
    public class DatabaseSettings
    {
        public string? UpdateServersCollectionName { get; set; }
        public string? LauncherCacheCollectionName { get; set; }
        public string? UpdateServerSessionCollectionName { get; set; }
        public string? AccountCollectionName { get; set; }
        public string? SessionCollectionName { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }
}