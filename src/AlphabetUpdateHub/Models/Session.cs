using System;

namespace AlphabetUpdateHub.Models
{
    public class Session
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public DateTime ExpireOn { get; set; }
        public string[] Servers { get; set; }
    }
}