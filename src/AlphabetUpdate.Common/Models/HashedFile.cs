﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthYouClient.Models
{
    [Serializable]
    [ObfuscationAttribute(Exclude=false, ApplyToMembers=true, Feature = "-rename;-typescramble")]
    public class HashedFile
    {
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }
        [JsonPropertyName("path")]
        public string? Path { get; set; }
    }
}
