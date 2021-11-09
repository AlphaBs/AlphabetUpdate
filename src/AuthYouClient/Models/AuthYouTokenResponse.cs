using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuthYouClient.Models
{
    [Serializable]
    [ObfuscationAttribute(Exclude=false, ApplyToMembers=true, Feature = "-rename;-typescramble")]
    public class AuthYouTokenResponse : AuthYouResponse
    {
        [JsonProperty("serverId")]
        public string ServerId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
