using AlphabetUpdate.Client.PatchProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Minecraft.PatchHandler
{
    public static class Extensions
    {
        public static PatchProcessBuilder AddGameOptionsPatcher(this PatchProcessBuilder b,
    GameOptions options)
        {
            b.AddHandler(new OptionsPatchHandler(options));
            return b;
        }

        public static PatchProcessBuilder AddShaderOptionsPatcher(this PatchProcessBuilder b,
            ShaderOptions options)
        {
            b.AddHandler(new ShaderOptionsPatchHandler(options));
            return b;
        }
    }
}
