namespace AlphabetUpdateHub.Models
{
    public class Account
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public string[] Servers { get; set; }
    }
}