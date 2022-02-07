using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Handlers
{
    public class FileFilterSetting
    {
        public bool FilterFromBasePath { get; set; } = false;
        public string[]? TargetDirectories { get; set; }
    }
}
