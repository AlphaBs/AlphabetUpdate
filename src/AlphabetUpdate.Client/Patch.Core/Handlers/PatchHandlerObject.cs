using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Handlers
{
    public class PatchHandlerObject : IPatchStep
    {
        private readonly IPatchHandler _patchHandler;

        public PatchHandlerObject(IPatchHandler patchHandler)
        {
            _patchHandler = patchHandler;
        }

        public IPatchHandler CreateHandler(PatchContext context)
        {
            return _patchHandler;
        }
    }
}
