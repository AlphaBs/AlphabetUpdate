using System;
using System.Collections.Generic;

namespace AlphabetUpdateServer.Models
{
    public class UpdateFileCollection
    {
        public DateTime LastUpdate { get; set; }
        public string? HashAlgorithm { get; init; }
        public IEnumerable<UpdateFile>? Files { get; init; }
    }
}
