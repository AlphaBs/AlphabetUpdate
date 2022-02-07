using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Services
{
    // IPatchService 객체를 생성하고 사용할 수 있도록 활성화
    public interface IPatchServiceActivator
    {
        IPatchService CreateService(PatchContext context);
    }
}
