using AlphabetUpdate.Client.PatchHandler;

namespace AlphabetUpdate.Client.PatchProcess
{
    public class PatchProcess
    {
        public PatchProcess(IPatchHandler[] patchers, PatchOptions options)
        {
            Patchers = patchers;
            Options = options;
        }

        public IPatchHandler[] Patchers { get; private set; }
        public PatchOptions Options { get; private set; }
    }
}