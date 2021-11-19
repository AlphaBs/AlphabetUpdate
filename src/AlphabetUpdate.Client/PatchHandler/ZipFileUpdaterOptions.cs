using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class ZipFileUpdaterOptions
    {
        public ZipFileUpdaterOptions(DateTime latest)
        {
            this.LatestVersion = latest;
        }

        public DateTime LatestVersion { get; set; }
        public string? LastUpdateFilePath { get; set; }
        public DateTime? LastUpdate { get; set; }
        public Stream? ZipStream { get; set; }
    }
}
