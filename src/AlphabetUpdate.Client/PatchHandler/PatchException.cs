using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class PatchException : Exception
    {
        public PatchException(string message) : base(message)
        {

        }
    }
}
