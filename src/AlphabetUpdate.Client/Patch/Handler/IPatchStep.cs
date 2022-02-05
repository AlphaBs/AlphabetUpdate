using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Handler
{
    public interface IPatchStep
    {
        IPatchHandler CreateHandler(PatchContext context);
    }
}
