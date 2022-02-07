using System;
using System.Collections.Generic;
using System.Text;

namespace AlphabetUpdate.Client.Patch.Core.Services
{
    public abstract class PatchServiceBase<T> : PatchServiceBase where T : class
    {
        private T? _setting; // nullable

        public T? Setting
        {
            get => _setting;
            set => _setting = value;
        }
    }
}
