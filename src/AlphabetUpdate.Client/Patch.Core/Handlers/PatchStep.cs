using AlphabetUpdate.Client.Patch.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Handlers
{
    public class PatchStep<THandler> : IPatchStep where THandler : PatchHandlerBase
    {
        public IPatchHandler CreateHandler(PatchContext context)
        {
            var service = ActivatorUtilities.GetServiceOrCreateInstance<THandler>(context.ServiceProvider);
            service.PatchContext = context;
            return service;
        }
    }
}
