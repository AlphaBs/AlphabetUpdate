using AlphabetUpdate.Client.Patch.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Services
{
    public class FileTagService : IFileTagService
    {
        public static readonly string TagIgnore = "ign";

        public FileTagCollection Tags { get; set; } = new FileTagCollection();

        public void AddIgnoreFile(string filepath)
        {
            Tags.AddFile(filepath, TagIgnore);
        }

        public bool CheckIgnoreFile(string filepath)
        {
            return Tags.HasTag(filepath, TagIgnore);
        }
    }
}
