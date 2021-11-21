using CmlLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client
{
    public class MinecraftLauncherCoreBuilder : LauncherCoreBuilder
    {
        protected readonly MinecraftPath minecraftPath;

        protected readonly List<Action<MLaunchOption>> launchOptionActions
    = new List<Action<MLaunchOption>>();

        public MinecraftLauncherCoreBuilder(MinecraftPath path) : base(path.BasePath)
        {
            this.minecraftPath = path;
        }

        public MinecraftLauncherCoreBuilder(string path) : base(path)
        {
            this.minecraftPath = new MinecraftPath(path);
        }

        public void AddLaunchOptionAction(Action<MLaunchOption> action)
        {
            launchOptionActions.Add(action);
        }

        public MinecraftLauncherCore BuildMinecraftLauncher()
        {
            var patchProcess = PatchProcess.Build();
            var launchOption = new MLaunchOption();
            launchOptionActions.ForEach(a => a.Invoke(launchOption));

            return new MinecraftLauncherCore(ClientPath,
                patchProcess,
                interactors.ToArray(),
                minecraftPath,
                launchOption);
        }
    }
}
