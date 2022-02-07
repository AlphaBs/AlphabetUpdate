using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Services
{
    public class PatchServiceActivator<T> : IPatchServiceActivator where T : PatchServiceBase
    {
        public IPatchService CreateService(PatchContext context)
        {
            var service = context.ServiceProvider.GetRequiredService<T>();
            service.PatchContext = context;
            return service;
        }
    }
}
