using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthYouClient
{
    public interface ITokenEncryptor
    {
        string Encrypt(string token);
    }
}
