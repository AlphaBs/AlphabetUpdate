using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthYouClient.Models
{
    public class AuthYouException : Exception
    {
        public AuthYouException(string message) : base(message)
        {

        }

        public AuthYouException(AuthYouResponse res) : base(res.Message)
        {
            this.Response = res;
        }

        public AuthYouException(string msg, Exception inner) : base(msg, inner)
        {
        }

        public AuthYouResponse Response { get; private set; }
    }
}
