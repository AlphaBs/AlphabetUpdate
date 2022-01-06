namespace AlphabetUpdateServer.Models
{
    public class ExternalServerOptions
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string Content { get; set; }
        public string AuthorizationScheme { get; set; }
        public string AuthorizationParameter { get; set; }
    }
}
