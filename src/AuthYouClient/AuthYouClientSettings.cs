using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthYouClient
{
    public class AuthYouClientSettings
    {
        public AuthYouClientApi? ClientApi { get; set; }
        public string? Uuid { get; set; }
        public ITokenEncryptor? TokenEncryptor { get; set; } = new TokenEncryptor();
        public string? BasePath { get; set; }
        public string[]? TargetDirs { get; set; }

        public int InteractorReadyDelaySec { get; set; } = 10;
        public int InteractorCheckingDelaySec { get; set; } = 1 * 60;
    }
}
