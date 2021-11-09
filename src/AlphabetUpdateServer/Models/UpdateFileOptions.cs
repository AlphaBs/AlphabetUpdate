using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Models
{
    public class UpdateFileOptions
    {
        public static readonly string UpdateFile = "UpdateFile";

        [Required]
        public string Name { get; set; }
        [Required]
        public string InputDir { get; set; }
        [Required]
        public string OutputDir { get; set; }
        [Required]
        public string BaseUrl { get; set; }
        [Required]
        public string LauncherInfoPath { get; set; }
        [Required]
        public string FilesCachePath { get; set; }
    }
}
