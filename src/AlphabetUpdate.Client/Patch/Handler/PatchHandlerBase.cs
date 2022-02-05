using AlphabetUpdate.Client.Patch.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Handler
{
    public abstract class PatchHandlerBase : PatchServiceBase, IPatchHandler
    {
        public override Task Initialize()
        {
            return Task.CompletedTask;
        }

        public abstract Task Patch(CancellationToken? cancellationToken);

        public virtual Task PostPatch(CancellationToken? cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}