using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class AlphabetFileUpdaterOptions
    {
        public string? BaseUrl { get; set; }
        public bool CheckHash { get; set; } = true;
        public int RetryCount { get; set; } = 3;
        public DateTime? LastUpdate { get; set; }
        public string? LastUpdateFilePath { get; set; }
        public string[]? AlwaysUpdates { get; set; }
        public bool SkipEmptyUrl { get; set; }
    }
}
