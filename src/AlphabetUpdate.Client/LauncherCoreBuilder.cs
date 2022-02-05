using AlphabetUpdate.Client.Patch;
using AlphabetUpdate.Client.PatchHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client
{
    public class LauncherCoreBuilder
    {
        public LauncherCoreBuilder(string path)
        {
            this.BasePath = path;
            this.PatchProcess = new PatchProcess();
        }

        public string BasePath { get; private set; }

        public PatchProcess PatchProcess { get; private set; }

        protected readonly List<ProcessManage.ProcessInteractor> interactors
            = new List<ProcessManage.ProcessInteractor>();

        public void AddProcessInteractor(ProcessManage.ProcessInteractor interactor)
        {
            interactors.Add(interactor);
        }

        public BaseLauncherCore Build()
        {
            return new BaseLauncherCore(
                BasePath,
                PatchProcess,
                interactors.ToArray());
        }
    }
}
