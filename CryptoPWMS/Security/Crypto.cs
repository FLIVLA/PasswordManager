using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using CryptoPWMS.IO;
using Konscious.Security.Cryptography;

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
        #region ========================= GENERATOR METHODS =========================

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

        #endregion

        #region =========================== VAULT METHODS ===========================

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vaultPath"></param>
        /// <param name="key"></param>
        public static void EncryptVault(string user_vaultname, byte[] key)
        {
            var tempDecr = Vaults.VaultPath(Vaults.VaultState.Decrypted_Temp, user_vaultname);
            var tempEncr = Vaults.VaultPath(Vaults.VaultState.Encrypted_Temp, user_vaultname);
            var finalDest = Vaults.VaultPath(Vaults.VaultState.Encrypted, user_vaultname); 

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (var ifs = File.OpenRead(tempDecr))
                using (var ofs = File.Create(tempEncr))
                {
                    ofs.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ofs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        ifs.CopyTo(cs);
                    }
                }
            }

            File.Copy(tempEncr, finalDest, true);
            File.Delete(tempDecr);
            File.Delete(tempEncr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_vaultname"></param>
        /// <param name="key"></param>
        public static void DecryptVault(string user_vaultname, byte[] key)
        {
            if (!Directory.Exists(Vaults.TempDir)) Directory.CreateDirectory(Vaults.TempDir);

            var encr = Vaults.VaultPath(Vaults.VaultState.Encrypted, user_vaultname);
            var tempEncr = Vaults.VaultPath(Vaults.VaultState.Encrypted_Temp, user_vaultname);
            var finalDest = Vaults.VaultPath(Vaults.VaultState.Decrypted_Temp, user_vaultname);

            File.Copy(encr, tempEncr);

            using (var ifs = File.OpenRead(tempEncr))
            using (var ofs = File.Create(finalDest)) // Create decrypted file
            {
                var aes = Aes.Create();
                var iv = new byte[aes.BlockSize / 8];
                ifs.Read(iv, 0, iv.Length);
                aes.Key = key;
                aes.IV = iv;

                using (var cs = new CryptoStream(ifs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cs.CopyTo(ofs);
                }
            }
        }

        #endregion

        #region =========================== AES METHODS ===========================

        public static byte[] Encrypt_AES(byte[] data, byte[] key, byte[] salt, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = key;
                aesAlg.IV = iv; // Use the provided IV

                // Use PBKDF2 to derive a key from the given salt and password
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt, 10000, HashAlgorithmName.SHA256);
                aesAlg.Key = pdb.GetBytes(32);

                // Set the padding mode
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(data, 0, data.Length);
                            csEncrypt.FlushFinalBlock();
                        }
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public static byte[] Decrypt_AES(byte[] encryptedData, byte[] key, byte[] salt, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = key;
                aesAlg.IV = iv; // Use the provided IV

                // Use PBKDF2 to derive a key from the given salt and password
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt, 10000, HashAlgorithmName.SHA256);
                aesAlg.Key = pdb.GetBytes(32);

                // Set the padding mode
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (MemoryStream msOriginal = new MemoryStream())
                {
                    csDecrypt.CopyTo(msOriginal);
                    return msOriginal.ToArray();
                }
            }
        }

        #endregion

        public static string Hash_Argon2(string password, byte[] salt) 
        {
            int itr = 10;
            int memprySize = 131072;
            int parallelism = 4;

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = parallelism;
                argon2.MemorySize = memprySize;
                argon2.Iterations = itr;

                byte[] hb = argon2.GetBytes(32);
                string hex = BitConverter.ToString(hb).Replace("-", "").ToLower();

                return hex;
            }
        }

        public static byte[] DeriveKey(string masterPassword, byte[] salt)
        {
            int itr = 12;
            int memorySize = 131072;
            int threads = 4;
            int outputLength = 32;

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(Hash_Argon2(masterPassword, salt))))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = threads;
                argon2.MemorySize = memorySize;
                argon2.Iterations = itr;

                return argon2.GetBytes(outputLength);
            }
        }

    }
}
