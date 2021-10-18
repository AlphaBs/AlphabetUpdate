using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServerInstaller
{
    public class AppSettings
    {
        public string SecureStorageKey { get; set; }
        public string SecureStorageIV { get; set; }
        public bool UseSecureAesStorage { get; set; }
        public SecureKeys SecureStorage { get; set; }
        public AuthOptions Auth { get; set; }
        public UpdateFileOptions UpdateFile { get; set; }
    }
}
