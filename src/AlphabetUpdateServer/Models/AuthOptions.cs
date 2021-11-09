using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Models
{
    public class AuthOptions
    {
        public static readonly string Auth = "Auth";

        [Required]
        public string Issuer { get; init; }
        
        [Required]
        [Range(0, 24 * 60)]
        public int ExpiresM { get; init; }
    }
}
