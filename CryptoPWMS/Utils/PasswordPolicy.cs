using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPWMS.Utils
{
    internal static class PasswordPolicy
    {
        public enum Eval { pass, fail };

        public static bool Evaluate(string s)
        {
            int digits = 0;
            int symbols = 0;
            int uppercase = 0;

            if (s.Length < 8)
                return false;

            var chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsDigit(chars[i]))
                    digits++;

                else if (char.IsSymbol(chars[i]))
                    symbols++;

                else if (char.IsUpper(chars[i]))
                    uppercase++;
            }

            return digits > 0 && symbols > 0 && uppercase > 0;
        }
    }
}
