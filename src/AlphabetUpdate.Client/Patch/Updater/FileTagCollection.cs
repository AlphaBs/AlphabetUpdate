using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Updater
{
    // Collection that stores filepath-tag data
    // provides methods return files using tag name, return tags using filepath
    public class FileTagCollection
    {
        // use two way dictionary as internal file<->tag storage
        // maybe there is a more effecient way to store file<->tag data

        // filepath -> tagname
        private readonly Dictionary<string, HashSet<string>> filePathStorage
            = new Dictionary<string, HashSet<string>>();

        // tagname -> filepath
        private readonly Dictionary<string, HashSet<string>> tagStorage
            = new Dictionary<string, HashSet<string>>();

        // add filepath and tagnames to filePathStorage and return successfully added tag names
        private IEnumerable<string> addFilePathStorage(string filepath, IEnumerable<string> tagnames)
        {
            HashSet<string> taglist;

            if (filePathStorage.TryGetValue(filepath, out taglist))
            {
                var addedTag = new List<string>();

                foreach (var tagname in tagnames)
                {
                    if (taglist.Contains(tagname))
                        continue;

                    taglist.Add(tagname);
                    addedTag.Add(tagname);
                }
                return addedTag;
            }
            else
            {
                taglist = new HashSet<string>(tagnames);
                filePathStorage.Add(filepath, taglist);
                return tagnames;
            }

        }

        private bool addTagStorage(string filepath, string tagname)
        {
            HashSet<string> filelist;

            if (tagStorage.TryGetValue(tagname, out filelist))
            {
                if (filelist.Contains(tagname))
                    return false;
                else
                {
                    filelist.Add(filepath);
                    return true;
                }
            }
            else
            {
                filelist = new HashSet<string> { tagname };
                tagStorage.Add(tagname, filelist);
                return true;
            }
        }

        public void AddFile(string filepath, string tagname)
        {
            AddFile(filepath, new string[] { tagname });
        }

        public void AddFile(string filepath, IEnumerable<string> tagnames)
        {
            var addedTag = addFilePathStorage(filepath, tagnames);

            foreach (var tagname in addedTag)
            {
                addTagStorage(filepath, tagname);
            }
        }

        public void RemoveFile(string filepath)
        {
            throw new NotImplementedException();
        }

        public string[]? GetTags(string filepath)
        {
            if (filePathStorage.TryGetValue(filepath, out var tags))
            {
                return tags.ToArray();
            }

            return null;
        }

        public bool HasTag(string filepath, string tagname)
        {
            var tags = GetTags(filepath);
            if (tags == null)
                return false;

            return tags.Contains(tagname);
        }

        public string[]? GetFiles(string tagname)
        {
            if (tagStorage.TryGetValue(tagname, out var list))
            {
                return list.ToArray();
            }

            return null;
        }
    }
}
