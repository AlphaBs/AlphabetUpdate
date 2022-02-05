using AlphabetUpdate.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace AlphabetUpdate.Client.Patch
{
    public class PatchContext : IPatchContext
    {
        public string BasePath { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public Dictionary<string, object> Datas { get; private set; } = new Dictionary<string, object>();

        public PatchContext(PatchOptions options, IServiceProvider serviceProvider)
        {
            BasePath = options.BasePath;
            ServiceProvider = serviceProvider;
        }
    }
}