using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Services
{
    public class PatchServiceActivator<TService> : IPatchServiceActivator
        where TService : notnull
    {
        public IPatchService CreateService(PatchContext context)
        {
            var service = context.ServiceProvider.GetRequiredService<TService>();
            if (service is IPatchService patchService)
            {
                if (patchService is PatchServiceBase patchServiceBase)
                {
                    patchServiceBase.PatchContext = context;
                    return patchServiceBase;
                }

                return patchService;
            }
            else
                throw new InvalidCastException(typeof(TService) + " cannot be IPatchService");
        }
    }
}
