using AlphabetUpdate.Client.Patch.Service;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Handler
{
    // 실제 패치 로직이 들어가는 곳
    public interface IPatchHandler : IPatchService
    {
        Task Patch(CancellationToken? cancellationToken);
        Task PostPatch(CancellationToken? cancellationToken);
    }
}