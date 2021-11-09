using System;

namespace AlphabetUpdate.Common.Models
{
    public class LauncherMetadata
    {
        public DateTime LastInfoUpdate { get; set; }
        public LauncherInfo Launcher { get; set; }
        public UpdateFileCollection Files { get; set; }
    }
}