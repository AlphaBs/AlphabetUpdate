using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AuthYouClient.Models
{
    [Serializable]
    [ObfuscationAttribute(Exclude=false, ApplyToMembers=true, Feature = "-rename;-typescramble")]
    public class AuthYouResponse
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("msg")]
        public string? Message { get; set; }
    }
}
