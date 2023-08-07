using System;
using System.Linq;
using System.Security.Cryptography;

namespace CryptoPWMS.Security
{
    /*
     *   _________                        __          
     *   \_   ___ \_______ ___.__._______/  |_  ____  
     *   /    \  \/\_  __ <   |  |\____ \   __\/  _ \ 
     *   \     \____|  | \/\___  ||  |_> >  | (  <_> )
     *    \______  /|__|   / ____||   __/|__|  \____/ 
     *           \/        \/     |__|                
     */

    /// <summary>
    /// Crypto is a static class, and servers as a helper class for all encryption
    /// related operations in the password manager. Has necessary methods to
    /// perform key and salt generation, AES and RSA encryption and decryption.
    /// </summary>
    internal static class Crypto
    {
        /// <summary>
        /// Generates random cryptographic key using the Advanced Encryption Standard (AES) algorithm.
        /// </summary>
        /// <returns>The generated cryptographic as byte array.</returns>
        public static byte[] GenerateKey()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key;
            }
        }

        /// <summary>
        /// Generates a random salt for cryptographic operations.
        /// </summary>
        /// <returns>Returns the generated salt as byte array.</returns>
        public static byte[] GenerateSalt() 
        {
            byte[] salt = new byte[16];                         // create new array to store the generated salt.
            using (var r = RandomNumberGenerator.Create())      // using instance of class RandomNumberGenerator to generate random bytes.
            {
                r.GetBytes(salt);                               // Fill the salt array with random bytes.
            }
            return salt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] Encrypt_AES(byte[] data, byte[] key, byte[] salt)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;                              // Set the key size to 256-bit
                aes.Key = key;                                  
                aes.Mode = CipherMode.CBC;
                aes.GenerateIV();
                var iv = aes.IV;

                // COMBINE WITH SALT:
                var w_Salt = new byte[data.Length + salt.Length];
                data.CopyTo(w_Salt, 0);
                salt.CopyTo(w_Salt, data.Length);

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new System.IO.MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(w_Salt, 0, w_Salt.Length);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encrData"></param>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] Decrypt_AES(byte[] encrData, byte[] key, byte[] salt)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                var iv = new byte[aes.BlockSize / 8];
                Array.Copy(encrData, iv, iv.Length);
                aes.IV = iv;

                using (var decr = aes.CreateDecryptor())
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decr, CryptoStreamMode.Write))
                    {
                        cs.Write(encrData, iv.Length, encrData.Length);
                        cs.FlushFinalBlock();
                        byte[] decr_wSalt = ms.ToArray();
                        byte[] decrData = new byte[decr_wSalt.Length - salt.Length];
                        byte[] decrSalt = new byte[salt.Length];

                        Array.Copy(decr_wSalt, 0, decrData, 0, decrData.Length);
                        Array.Copy(decr_wSalt, decrData.Length, decrSalt, 0, decrSalt.Length);

                        if (!salt.SequenceEqual(decrSalt)) {
                            throw new CryptographicException("Invalid salt!");
                        }

                        return decrData;
                    }   
                }
            }
        }
    }
}
