using System;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Core.Services
{
    public abstract class PatchServiceBase : IPatchService
    {
        private PatchContext? _patchContext;

        public PatchContext PatchContext
        {
            get => _patchContext ?? throw new InvalidOperationException("not activated");
            set => _patchContext = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string BasePath => PatchContext.BasePath;

        public abstract Task Initialize();
    }
}
