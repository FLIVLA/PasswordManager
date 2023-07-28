using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPWMS.Models
{
    public class PasswordGenerator
    {
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string Symbols = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        private int _length;
        private bool _useCaps;
        private bool _UseDigits;
        private bool _useSymbols;
        private bool _grpChars;

        private Random Random = new Random();

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
            
        }

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

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
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
