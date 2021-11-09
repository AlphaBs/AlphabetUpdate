namespace AlphabetUpdateServer.Models
{
    public class SecureKeys
    {
        // login hash(password + salt)
        public string? HashedPassword { get; init; }
        // login password salt
        public string? Salt { get; init; }

        // login AES key
        public string? AesKey { get; init; }
        // login AES IV
        public string? AesIV { get; init; }

        // JWT Secret Key
        public string? SecretKey { get; init; }
    }
}
