using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Service
{
    public class PatchServiceActivator<TService, TSetting> : IPatchServiceActivator
        where TService : PatchServiceBase<TSetting>
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
            service.PatchContext = context;
            service.Setting = _setting;
            return service;
        }
    }
}
