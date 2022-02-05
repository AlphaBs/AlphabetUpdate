using AlphabetUpdate.Client.Patch.Handler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch
{
    // PatchProcess 를 실제로 실행하는 클래스
    // TODO: 예외처리
    public class PatchProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public PatchProcessor(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // PatchProcess 에 있는 PatchStep 을 모두 수행
        public async Task Patch(PatchProcess patchProcess, PatchOptions options, CancellationToken? cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var patchContext = new PatchContext(options, scope.ServiceProvider);

            await startPatchServices(patchProcess, patchContext);
            await startPatchHandlers(patchProcess, patchContext, cancellationToken);
        }

        private async Task startPatchServices(PatchProcess patchProcess, PatchContext patchContext)
        {
            foreach (var serviceActivator in patchProcess.GetPatchServices())
            {
                var service = serviceActivator.CreateService(patchContext);
                await service.Initialize();
            }
        }

        private async Task startPatchHandlers(PatchProcess patchProcess, PatchContext patchContext, CancellationToken? cancellationToken)
        {
            var handlers = new List<IPatchHandler>();
            foreach (var patchStep in patchProcess.GetPatchSteps())
            {
                var handler = patchStep.CreateHandler(patchContext);
                handlers.Add(handler);
            }

            // Initialize
            foreach (var handler in handlers)
            {
                await handler.Initialize();
            }

            // Patch
            foreach (var handler in handlers)
            {
                await handler.Patch(cancellationToken);
            }

            // PostPatch
            foreach (var handler in handlers)
            {
                await handler.PostPatch(cancellationToken);
            }
        }
    }
}
