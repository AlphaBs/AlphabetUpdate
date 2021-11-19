using AlphabetUpdate.Client.PatchHandler;
using CmlLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client
{
    public class LauncherCoreBuilder
    {
        public LauncherCoreBuilder(MinecraftPath path)
        {
            this.MinecraftPath = path;
        }

        public MinecraftPath MinecraftPath { get; private set; }

        public PatchProcess.PatchProcessBuilder PatchProcess { get; private set; }
            = new PatchProcess.PatchProcessBuilder();

        private readonly List<ProcessManage.ProcessInteractor> interactors
            = new List<ProcessManage.ProcessInteractor>();

        private readonly List<Action<MLaunchOption>> launchOptionActions
            = new List<Action<MLaunchOption>>();

        public void AddProcessInteractor(ProcessManage.ProcessInteractor interactor)
        {
            interactors.Add(interactor);
        }

        public void AddLaunchOptionAction(Action<MLaunchOption> action)
        {
            launchOptionActions.Add(action);
        }

        public LauncherCore Build()
        {
            var patchProcess = PatchProcess.Build();
            var launchOption = new MLaunchOption();
            launchOptionActions.ForEach(a => a.Invoke(launchOption));

            return new LauncherCore(MinecraftPath, 
                patchProcess, 
                interactors.ToArray(),
                launchOption);
        }
    }
}
