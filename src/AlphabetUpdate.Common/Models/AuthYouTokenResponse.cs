using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthYouClient.Models
{
    [Serializable]
    [ObfuscationAttribute(Exclude=false, ApplyToMembers=true, Feature = "-rename;-typescramble")]
    public class AuthYouTokenResponse : AuthYouResponse
    {
        [JsonPropertyName("serverId")]
        public string? ServerId { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}
