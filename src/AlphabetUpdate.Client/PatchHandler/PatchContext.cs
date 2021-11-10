using System;
using System.Collections.Generic;
using System.IO;
using AlphabetUpdate.Common.Helpers;
using CmlLib.Core;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class PatchContext
    {
        public MinecraftPath MinecraftPath { get; private set; }
        public event EventHandler<string>? StatusChanged; 

        private readonly Dictionary<string, List<string>> tagFileDict;
        private readonly HashSet<string>? ignoreTags;
        private readonly HashSet<string>? tags;
        private readonly HashSet<string>? whitelistDirs;
        private readonly HashSet<string>? whitelistFiles;

        public PatchContext(PatchOptions options)
        {
            if (options.MinecraftPath == null)
                throw new InvalidOperationException("MinecraftPath was null");
            
            MinecraftPath = options.MinecraftPath;
            tagFileDict = new Dictionary<string, List<string>>();
            
            if (options.WhitelistDirs != null && options.WhitelistDirs.Length > 0)
            {
                whitelistDirs = new HashSet<string>();
                foreach (var dir in options.WhitelistDirs)
                {
                    whitelistDirs.Add(IoHelper.NormalizePath(Path.Combine(
                        options.MinecraftPath.BasePath, dir)));
                }
            }

            if (options.WhitelistFiles != null && options.WhitelistFiles.Length > 0)
            {
                whitelistFiles = new HashSet<string>();
                foreach (var file in options.WhitelistFiles)
                {
                    whitelistFiles.Add(IoHelper.NormalizePath(Path.Combine(
                        options.MinecraftPath.BasePath, file)));
                }
            }

            if (options.Tags != null && options.Tags.Length > 0)
                tags = new HashSet<string>(options.Tags);
        }

        public List<string> GetTagFilePathList(string tag)
        {
            if (!tagFileDict.ContainsKey(tag))
                tagFileDict[tag] = new List<string>();
            return tagFileDict[tag];
        }
        
        public bool IsWhitelistDir(string fullPath)
        {
            if (whitelistDirs == null)
                return true;
            return whitelistDirs.Contains(fullPath);
        }

        public bool IsWhitelistFile(string fullPath)
        {
            if (whitelistFiles == null)
                return true;
            return whitelistFiles.Contains(fullPath);
        }

        public bool IsTagContains(string tag)
        {
            if (tags == null)
                return true;
            return tags.Contains(tag);
        }

        public bool IsIgnoreTagContains(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return true;
            return tag == "common" || tag == "forge";
        }

        public virtual void SetStatus(string message)
        {
            StatusChanged?.Invoke(this, message);
        }
    }
}