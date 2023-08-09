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
    /// Used for generating strong passwords, using the user's desired settings
    /// for the composition of the generated output.
    /// </summary>
    public class PasswordGenerator
    {
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";   // Allowed UpperCase letters.
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";   // Allowed LowerCase letters.
        private const string Digits = "0123456789";                             // Allowed digits.
        private const string Symbols = "!@#$%^&*()_=+[]{}|?";                   // Allowed Symbols.

        public int Length;        // desired length of the password to be generated.
        public bool UseCaps;      // Boolean representation of whether the generator instance will use upper case letters.
        public bool UseDigits;    // Boolean representation of whether the generator instance will use digits.
        public bool UseSymbols;   // Boolean representation of whether the generator instance will use symbols.
        public bool GrpChars;     // Boolean representation of whether the generator instance will group chars.

        /// <summary>
        /// Initializes with an password length of 20 chars.
        /// </summary>
        public PasswordGenerator()
        {
            Length = 20;
        }

        /// <summary>
        /// Generates a new random password with the current composition settings.
        /// </summary>
        /// <returns>Randomly generated password</returns>
        public string Generate()
        {
            StringBuilder password = new StringBuilder();        // new stringbuilder instance for appending random characters.
            string characterSet = LowercaseLetters;              // Initialize characterset as only lowercase letters as default.

            if (UseCaps) characterSet += UppercaseLetters;       // If UseCaps == true, add Uppercase letters to characterset.
            if (UseDigits) characterSet += Digits;               // If UseDigits == true, add digit characters to characterset.          
            if (UseSymbols) characterSet += Symbols;             // If UseSymbols == true, add allowed symbols to characterset.

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[Length];                  // initialize new byte array for holding the random bytes.
                rng.GetBytes(randomBytes);                              // This fills the array with random bytes.

                for (int i = 0; i < Length; i++)                        // Iterates over the current intended password length
                {
                    int index = randomBytes[i] % characterSet.Length;   // calculate index based on random byte value
                    password.Append(characterSet[index]);               // Append the char of the characterset at calculated index to the stringbuilder.
                }
            }

            if (GrpChars)                                          // If character grouping == true
            {
                int groupSize = 4;                                 // set the group size
                int passwordLength = password.Length;
                for (int i = groupSize; i < passwordLength; i += groupSize + 1)     // iterate the length of the password
                {
                    password.Insert(i, "-");                       // insert a hyphen character at i in the stringbuilder
                }
            }

            return password.ToString();                             
        }
    }
}
