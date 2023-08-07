using System.Security.Cryptography;
using System.Text;

namespace CryptoPWMS.Security
{
    internal class Hash
    {
        /// <summary>
        /// Computes SHA-256 hash of input string s.
        /// </summary>
        /// <param name="s">Input string to be hashed</param>
        /// <returns>SHA-256 hash of input string s, as hexadedecimal string.</returns>
        public static string ToSHA_256(string s)
        {
            var sha = SHA256.Create();
            byte[] b = sha.ComputeHash(Encoding.UTF8.GetBytes(s));   
            var sb = new StringBuilder();  
            
            for (int i = 0; i < b.Length; i++) {
                sb.Append(b[i].ToString("x2"));
            }
            
            return sb.ToString();
        }
    }
}
