using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuthYouClient.Models
{
    public class AuthYouResponse
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}
