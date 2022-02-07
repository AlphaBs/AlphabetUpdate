using AlphabetUpdate.Client.Patch.Core.Handlers;
using AlphabetUpdate.Client.Patch.Core.Services;
using System.Collections;
using System.Collections.Generic;

namespace AlphabetUpdate.Client.Patch.Core
{
    // PatchStep collection 저장
    // Extension method 이용해서 이 클래스에 Add~~~ 메서드 추가

    public class PatchProcess
    {
        private readonly List<IPatchServiceActivator> services = new List<IPatchServiceActivator>();
        private readonly List<IPatchStep> steps = new List<IPatchStep>();

        // IPatchHandler

        public void AddPatchStep(IPatchStep step)
        {
            steps.Add(step);
        }

        public void AddPatchHandler(IPatchHandler handler)
        {
            AddPatchStep(new PatchHandlerObject(handler));
        }

        public void AddPatchHandler<THandler>() where THandler : PatchHandlerBase
        {
            AddPatchStep(new PatchStep<THandler>());
        }

        public void AddPatchHandler<THandler, TSetting>(TSetting setting) 
            where THandler : PatchHandlerBase<TSetting>
            where TSetting : class
        {
            AddPatchStep(new PatchStep<THandler, TSetting>(setting));
        }

        // IPatchService

        public void AddPatchServiceActivator(IPatchServiceActivator service)
        {
            services.Add(service);
        }

        public void AddPatchService(IPatchService service)
        {
            AddPatchServiceActivator(new PatchServiceObject(service));
        }

        public void AddPatchService<TService>() where TService : PatchServiceBase
        {
            AddPatchServiceActivator(new PatchServiceActivator<TService>());
        }

        public void AddPatchService<TService, TSetting>(TSetting setting)
            where TService : PatchServiceBase<TSetting>
            where TSetting : class
        {
            AddPatchServiceActivator(new PatchServiceActivator<TService, TSetting>(setting));
        }

        public IEnumerable<IPatchStep> GetPatchSteps()
        {
            return steps;
        }

        public IEnumerable<IPatchServiceActivator> GetPatchServices()
        {
            return services;
        }
    }
}