using System;
using System.Collections.Generic;

namespace AlphabetUpdate.Client.Patch
{
    public interface IPatchContext
    {
        string BasePath { get; }
        IServiceProvider ServiceProvider { get; }

        // common methods
        Dictionary<string, object> Datas { get; }
    }
}
