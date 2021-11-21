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
            this.ClientPath = path;
            this.PatchProcess = new PatchProcess.PatchProcessBuilder(path);
        }

        public string ClientPath { get; private set; }

        public PatchProcess.PatchProcessBuilder PatchProcess { get; private set; }

        protected readonly List<ProcessManage.ProcessInteractor> interactors
            = new List<ProcessManage.ProcessInteractor>();

        public void AddProcessInteractor(ProcessManage.ProcessInteractor interactor)
        {
            interactors.Add(interactor);
        }

        public BaseLauncherCore Build()
        {
            return new BaseLauncherCore(
                ClientPath,
                PatchProcess.Build(),
                interactors.ToArray());
        }
    }
}
