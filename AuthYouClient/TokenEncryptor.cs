using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthYouClient.Models;

namespace AuthYouClient
{
    public class TokenEncryptor : ITokenEncryptor
    {
        public const int MaxTokenLength = 512;

        public string Encrypt(string tokenStr)
        {
            if (tokenStr.Length > MaxTokenLength)
                throw new AuthYouException("Exceed max token length");

            var plain = Convert.FromBase64String(tokenStr);
            var encBuffer = new byte[plain.Length];

            int preValue = (int)Math.Pow(2, 8);
            byte byteMaker = 255;
            for (int i = 0; i < plain.Length; i++)
            {
                byte b = (byte)Math.Abs(plain[i]);
                byte last2 = (byte)((b << 6) & byteMaker);
                b = (byte)((b >> 2) & byteMaker);
                b |= last2;

                if (preValue <= 0)
                    preValue = 1;
                
                var newB = b % preValue;
                if (newB <= 0)
                    newB = b;

                encBuffer[i] = (byte)newB;
                preValue = encBuffer[i];
            }

            return Convert.ToBase64String(encBuffer);
        }
    }
}
