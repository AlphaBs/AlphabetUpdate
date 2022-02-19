using AlphabetUpdate.Client.Patch.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Handlers
{
    public class PatchStep<THandler, TSetting> : IPatchStep
        where THandler : PatchHandlerBase<TSetting>
        where TSetting : class
    {
        private readonly TSetting _setting;

        public PatchStep(TSetting setting)
        {
            _setting = setting;
        }

        public IPatchHandler CreateHandler(PatchContext context)
        {
            var service = ActivatorUtilities.GetServiceOrCreateInstance<THandler>(context.ServiceProvider);
            service.PatchContext = context;
            service.Setting = _setting;
            return service;
        }
    }
}
