using System;
using System.Collections.Generic;

namespace AlphabetUpdate.Common.Models
{
    public class UpdateFileCollection
    {
        public DateTime LastUpdate { get; set; }
        public string HashAlgorithm { get; set; }
        public UpdateFile[] Files { get; set; }
    }
}