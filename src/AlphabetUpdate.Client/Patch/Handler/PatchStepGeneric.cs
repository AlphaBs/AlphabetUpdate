using AlphabetUpdate.Client.Patch.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Handler
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
            var activator = new PatchServiceActivator<THandler, TSetting>(_setting);
            return (THandler)activator.CreateService(context);
        }
    }
}
