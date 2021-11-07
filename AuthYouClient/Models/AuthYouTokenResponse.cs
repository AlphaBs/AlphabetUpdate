using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuthYouClient.Models
{
    public class AuthYouTokenResponse : AuthYouResponse
    {
        [JsonProperty("serverId")]
        public string ServerId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
