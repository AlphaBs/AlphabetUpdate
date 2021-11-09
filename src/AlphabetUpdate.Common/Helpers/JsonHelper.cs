using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Common.Helpers
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}