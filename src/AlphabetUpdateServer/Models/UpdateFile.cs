namespace AlphabetUpdateServer.Models
{
    public class UpdateFile
    {
        public string? Url { get; init; }
        public string? Path { get; init; }
        public string? Hash { get; init; }
        public string? Tags { get; init; }
    }
}