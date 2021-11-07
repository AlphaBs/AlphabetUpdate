﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuthYouClient.Models
{
    public class HashedFile
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
