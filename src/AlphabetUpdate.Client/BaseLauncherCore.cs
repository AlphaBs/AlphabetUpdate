using AlphabetUpdate.Client.Patch;
using AlphabetUpdate.Client.ProcessManage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client
{
    // 패치, 실행, 프로세스 관리
    public class BaseLauncherCore
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly string BasePath;
        protected readonly PatchOptions PatchOptions;
        public ProcessInteractor[]? ProcessInteractors { get; private set; }
        public PatchProcess PatchProcess { get; private set; }

        public BaseLauncherCore(
            string path, IServiceProvider serviceProvider, 
            PatchProcess patch, PatchOptions patchOptions, 
            ProcessInteractor[]? interactors)
        {
            ServiceProvider = serviceProvider;

            BasePath = path;
            PatchProcess = patch;
            PatchOptions = patchOptions;
            ProcessInteractors = interactors;
        }

        public virtual async Task Patch(CancellationToken cancellationToken)
        {
            var processor = new PatchProcessor(ServiceProvider);
            await processor.Patch(PatchProcess, PatchOptions, cancellationToken).ConfigureAwait(false);
        }
    }
}
