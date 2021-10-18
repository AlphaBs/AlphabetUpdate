using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Models
{
    public class UpdateFileOptions
    {
        public static readonly string UpdateFile = "UpdateFile";

        [Required]
        public string Name { get; init; }
        [Required]
        public string Root { get; init; }
        [Required]
        public string Path { get; init; }
        [Required]
        public string LauncherInfoPath { get; init; }
        [Required]
        public string FilesCachePath { get; init; }
    }
}
