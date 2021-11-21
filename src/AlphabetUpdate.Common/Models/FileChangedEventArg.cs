using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Common.Models
{
    public delegate void FileChangedEventHandler(object sender, FileChangedEventArg e);

    public class FileChangedEventArg
    {
        public string? NowFileType { get; set; }
        public string? NowFileName { get; set; }
        public int TotalFileCount { get; set; }
        public int ProgressedFileCount { get; set; }
    }
}
