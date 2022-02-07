using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Services
{
    public interface IFileTagService
    {
        FileTagCollection Tags { get; }
    }
}
