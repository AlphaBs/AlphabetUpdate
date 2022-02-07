using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Services
{
    public class WhitelistFileSetting
    {
        public string[]? WhiteFiles { get; set; }
        public string[]? WhiteDirs { get; set; }
    }
}
