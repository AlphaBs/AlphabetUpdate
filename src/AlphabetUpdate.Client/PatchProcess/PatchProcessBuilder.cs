using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;

namespace AlphabetUpdate.Client.PatchProcess
{
    public class PatchProcessBuilder : IPatchProcessBuilder
    {
        public PatchProcessBuilder(string path)
        {
            patchOptions = new PatchOptions(path);
        }

        public virtual PatchProcess Build()
        {
            var patchProcess = new PatchProcess(handlers.ToArray(), patchOptions);
            return patchProcess;
        }

        protected readonly List<IPatchHandler> handlers = new List<IPatchHandler>();
        protected readonly PatchOptions patchOptions;

        public PatchProcessBuilder AddHandler(IPatchHandler handler)
        {
            handlers.Add(handler);
            return this;
        }

        public PatchProcessBuilder SetOptions(Action<PatchOptions> fn)
        {
            fn(patchOptions);
            return this;
        }
    }
}