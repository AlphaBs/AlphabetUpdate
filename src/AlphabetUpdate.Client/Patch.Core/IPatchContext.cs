using System;
using System.Collections.Generic;

namespace AlphabetUpdate.Client.Patch.Core
{
    public interface IPatchContext
    {
        string BasePath { get; }
        IServiceProvider ServiceProvider { get; }

        // common methods
        Dictionary<string, object> Items { get; }
    }
}
