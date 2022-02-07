using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Services
{
    public class PatchServiceObject : IPatchServiceActivator
    {
        private readonly IPatchService _service;

        public PatchServiceObject(IPatchService service)
        {
            _service = service;
        }

        public IPatchService CreateService(PatchContext context)
        {
            return _service;            
        }
    }
}
