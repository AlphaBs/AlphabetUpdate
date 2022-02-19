using System;

namespace AlphabetUpdate.Client.Patch.Core
{
    public class PatchException : Exception
    {
        public PatchException(string message) : base(message)
        {

        }

        public PatchException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
