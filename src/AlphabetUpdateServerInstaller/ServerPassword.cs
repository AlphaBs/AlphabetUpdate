namespace AlphabetUpdateServerInstaller
{
    public record ServerPassword
    {
        public string RawPassword { get; init; }
        public string AesKey { get; init; }
        public string AesIv { get; init; }
    }
}