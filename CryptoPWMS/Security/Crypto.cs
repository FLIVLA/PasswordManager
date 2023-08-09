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
    /// perform key and salt generation, AES encryption and decryption, and hashing
    /// methods using argon2 hashing algorithm implementation.
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
        /// Encrypts the user's vault using AES encryption with the provided key.
        /// </summary>
        /// <param name="vaultPath">The name of the user's vault.</param>
        /// <param name="key">The encryption key used for encryption. (Master key).</param>
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
        /// Decrypts the user's vault using AES decryption with the provided key. (Master key)
        /// </summary>
        /// <param name="user_vaultname">The name of the user's vault.</param>
        /// <param name="key">The decryption key used for decryption.</param>
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

        /// <summary>
        /// Encrypts the provided data using AES encryption with Argon2-derived key.
        /// </summary>
        /// <param name="data">Input data for encryption.</param>
        /// <param name="key">The key derived using Argon2.</param>
        /// <param name="salt">The salt used for Argon2 key derivation.</param>
        /// <param name="iv">The initialization vector for AES encryption.</param>
        /// <returns>The encrypted data.</returns>
        public static byte[] Encrypt_AES(byte[] data, byte[] key, byte[] salt, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())                               
            {
                aesAlg.KeySize = 256;

                using (var argon2 = new Argon2id(key))
                {
                    argon2.Iterations = 20;     
                    argon2.DegreeOfParallelism = 2;
                    argon2.MemorySize = 8192;
                    argon2.Salt = salt;
                    aesAlg.Key = argon2.GetBytes(32);
                }

                aesAlg.IV = iv;                                            
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

        /// <summary>
        /// Decrypts the provided encrypted data using AES decryption with Argon2-derived key.        
        /// </summary>
        /// <param name="encryptedData">The encrypted data to be decrypted.</param>
        /// <param name="key">The key derived using Argon2.</param>
        /// <param name="salt">The salt used for Argon2 key derivation.</param>
        /// <param name="iv">The initialization vector for AES decryption.</param>
        /// <returns>The decrypted data.</returns>
        public static byte[] Decrypt_AES(byte[] encryptedData, byte[] key, byte[] salt, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;

                using (var argon2 = new Argon2id(key))
                {
                    argon2.Iterations = 20;
                    argon2.DegreeOfParallelism = 2;
                    argon2.MemorySize = 8192;
                    argon2.Salt = salt;
                    
                    aesAlg.Key = argon2.GetBytes(32);
                }

                aesAlg.IV = iv;                                             
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
        #region ===================== ARGON 2 IMPLEMENTATION ========================

        /// <summary>
        /// Creates Argon2id hash of the provided password and salt, 
        /// and returns the hash as a lowercase hexadecimal string.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt to use for hashing.</param>
        /// <returns>The Argon2id hash as hexadecimal string</returns>
        public static string Hash_Argon2_HEX(string password, byte[] salt) 
        {
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 2;
                argon2.MemorySize = 8192;
                argon2.Iterations = 20;

                byte[] hb = argon2.GetBytes(32);
                string hex = BitConverter.ToString(hb).Replace("-", "").ToLower();
                return hex;
            }
        }

        /// <summary>
        /// Derives a key from the provided master password and salt 
        /// using the Argon2id key derivation function.
        /// </summary>
        /// <param name="masterPassword">Master password to derive key from.</param>
        /// <param name="salt">Master salt (stored in database).</param>
        /// <returns>The derived key as array of bytes.</returns>
        public static byte[] DeriveKey(string masterPassword, byte[] salt)
        {
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(masterPassword)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 2;
                argon2.MemorySize = 8192;
                argon2.Iterations = 20;

                return argon2.GetBytes(32);
            }
        }

        #endregion
    }
}
