using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlphabetUpdate.Client.PatchHandler;
using CmlLib.Core;

namespace AlphabetUpdate.Client.PatchProcess
{
    public class PatchProcessBuilder : IPatchProcessBuilder
    {
        public virtual Task<PatchProcess> Build()
        {
            var patchProcess = new PatchProcess(handlers.ToArray(), patchOptions);
            return Task.FromResult(patchProcess);
        }

        protected readonly List<IPatchHandler> handlers = new List<IPatchHandler>();
        protected readonly PatchOptions patchOptions = new PatchOptions();

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