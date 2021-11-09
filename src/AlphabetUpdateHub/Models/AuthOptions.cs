namespace AlphabetUpdateHub.Models
{
    public class AuthOptions
    {
        public static readonly string AuthOptionName = "Auth";
        
        public string Issuer { get; set; }
        public string SecretKey { get; set; }
        public int ExpiresM { get; set; }
    }
}