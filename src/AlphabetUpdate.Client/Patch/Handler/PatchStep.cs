using AlphabetUpdate.Client.Patch.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Handler
{
    public class PatchStep<THandler> : IPatchStep where THandler : PatchHandlerBase
    {
        public IPatchHandler CreateHandler(PatchContext context)
        {
            var serviceActivator = new PatchServiceActivator<THandler>();
            return (IPatchHandler)serviceActivator.CreateService(context);
        }
    }
}
