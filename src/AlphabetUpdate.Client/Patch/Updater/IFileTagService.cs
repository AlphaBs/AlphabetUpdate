using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Updater
{
    public interface IFileTagService
    {
        FileTagCollection Tags { get; }
    }
}
