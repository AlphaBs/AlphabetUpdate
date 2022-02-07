using AlphabetUpdate.Client.Patch.Services;
using AlphabetUpdate.Common.Models;
using System;
using System.Collections.Generic;

namespace AlphabetUpdate.Client.Patch.Handlers
{
    public class AlphabetFileUpdateSetting
    {
        public string? BaseUrl { get; set; }
        public bool CheckHash { get; set; } = true;
        public int RetryCount { get; set; } = 3;
        public DateTime? LastUpdate { get; set; }
        public string? LastUpdateFilePath { get; set; }
        public string[]? AlwaysUpdates { get; set; }
        public bool SkipEmptyUrl { get; set; }
        public UpdateFileCollection? UpdateFiles { get; set; }

        public FileTagOptions DefaultFileTagOption { get; set; } = FileTagOptions.Disable;
        public Dictionary<string, FileTagOptions>? TagOptions { get; set; }
        public string[]? Tags { get; set; }
        public bool SaveFileTags { get; set; }
        public bool SaveUpdateResultTag { get; set; }
    }
}
