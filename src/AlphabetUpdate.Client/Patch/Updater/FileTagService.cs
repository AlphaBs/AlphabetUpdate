using AlphabetUpdate.Client.Patch.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Updater
{
    public class FileTagService : IPatchService, IFileTagService
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

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
