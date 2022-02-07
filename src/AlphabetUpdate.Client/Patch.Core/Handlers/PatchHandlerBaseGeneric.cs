using AlphabetUpdate.Client.Patch.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Core.Handlers
{
    public abstract class PatchHandlerBase<T> : PatchServiceBase<T>, IPatchHandler where T : class
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
