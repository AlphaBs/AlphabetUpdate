using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Services
{
    public class PatchServiceActivator<TService, TSetting> : IPatchServiceActivator
        where TService : notnull
        where TSetting : class
    {
        private readonly TSetting _setting;

        public PatchServiceActivator(TSetting setting)
        {
            _setting = setting;
        }

        public IPatchService CreateService(PatchContext context)
        {
            var service = context.ServiceProvider.GetRequiredService<TService>();
            if (service is PatchServiceBase<TSetting> patchService)
            {
                patchService.PatchContext = context;
                patchService.Setting = _setting;
                return patchService;
            }
            else
                throw new InvalidCastException($"{typeof(TService)} cannot be PatchServiceBase<{typeof(TSetting)}>");
        }
    }
}
