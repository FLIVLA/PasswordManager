using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoPWMS.Security
{
    /*
     *   __________                                               .___  
     *   \______   \_____    ______ ________  _  _____________  __| _/  
     *   |     ___/\__  \  /  ___//  ___/\ \/ \/ /  _ \_  __ \/ __ |   
     *   |    |     / __ \_\___ \ \___ \  \     (  <_> )  | \/ /_/ |   
     *   |____|    (____  /____  >____  >  \/\_/ \____/|__|  \____ |   
     *                  \/     \/     \/                          \/   
     *     ________                                   __                
     *    /  _____/  ____   ____   ________________ _/  |_  ___________ 
     *   /   \  ____/ __ \ /    \_/ __ \_  __ \__  \\   __\/  _ \_  __ \
     *   \    \_\  \  ___/|   |  \  ___/|  | \// __ \|  | (  <_> )  | \/
     *    \______  /\___  >___|  /\___  >__|  (____  /__|  \____/|__|   
     *           \/     \/     \/     \/           \/                   
     */

    /// <summary>
    /// 
    /// </summary>
    public class PasswordGenerator
    {
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";   // Allowed UpperCase letters.
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";   // Allowed LowerCase letters.
        private const string Digits = "0123456789";                             // Allowed digits.
        private const string Symbols = "!@#$%^&*()_=+[]{}|?";                   // Allowed Symbols.

        private int _length;        // desired length of the password to be generated.
        private bool _useCaps;      // Boolean representation of whether the generator instance will use upper case letters.
        private bool _UseDigits;    // Boolean representation of whether the generator instance will use digits.
        private bool _useSymbols;   // Boolean representation of whether the generator instance will use symbols.
        private bool _grpChars;     // Boolean representation of whether the generator instance will group chars.

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public bool UseCaps
        {
            get { return _useCaps; }
            set { _useCaps = value; }
        }

        public bool UseDigits
        {
            get { return _UseDigits; }
            set { _UseDigits = value; }
        }

        public bool UseSymbols
        {
            get { return _useSymbols; }
            set { _useSymbols = value; }
        }

        public bool GrpChars
        {
            get { return _grpChars; }
            set { _grpChars = value; }
        }


        public PasswordGenerator()
        {
            _length = 20;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Generate()
        {
            StringBuilder password = new StringBuilder();
            string characterSet = LowercaseLetters;

            if (_useCaps)
                characterSet += UppercaseLetters;

            if (_UseDigits)
                characterSet += Digits;

            if (_useSymbols)
                characterSet += Symbols;

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[_length];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < _length; i++)
                {
                    int index = randomBytes[i] % characterSet.Length;
                    password.Append(characterSet[index]);
                }
            }

            if (_grpChars)
            {
                int groupSize = 4;
                int passwordLength = password.Length;
                for (int i = groupSize; i < passwordLength; i += groupSize + 1)
                {
                    password.Insert(i, "-");
                }
            }

            return password.ToString();
        }
    }
}
