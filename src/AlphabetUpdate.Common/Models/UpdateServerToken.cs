using System;

namespace AlphabetUpdate.Common.Models
{
    public class UpdateServerToken
    {
        public string ServerId { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}