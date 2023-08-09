using System;
using System.IO;
using System.Windows;

namespace CryptoPWMS.IO
{
    /*
     *   ____   ____            .__   __          
     *   \   \ /   /____   __ __|  |_/  |_  ______
     *    \   Y   /\__  \ |  |  \  |\   __\/  ___/
     *     \     /  / __ \|  |  /  |_|  |  \___ \ 
     *      \___/  (____  /____/|____/__| /____  >
     *                  \/                     \/ 
     */

    /// <summary>
    /// Helper class for password vault functionality.
    /// </summary>
    internal static class Vaults
    {
        public enum VaultState { Encrypted, Encrypted_Temp, Decrypted_Temp }

        public static readonly string BaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
        public static readonly string TemplateDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vault_Template");
        public static readonly string TempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

        public static string VaultPath(VaultState state, string current_user)
        {
            switch (state)
            {
                case VaultState.Encrypted:
                    return Path.Combine(BaseDir, current_user + ".db.cryptovault");
                
                case VaultState.Encrypted_Temp:
                    return Path.Combine(TempDir, current_user + ".db.cryptovault");

                case VaultState.Decrypted_Temp:
                    return Path.Combine(TempDir, current_user + ".db");
                
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Creates a new vault instance in the application base directory.
        /// Containing the necessary tables for password vault.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="masterPassword"></param>
        public static void Create(string dbName)
        {
            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

            if (File.Exists(Path.Combine(BaseDir, dbName + ".db"))) {
                MessageBox.Show("Database of that name already exist.");
                return;
            }
            try
            {
                string template = Path.Combine(TemplateDir, "vault_template.db");
                string newVault = Path.Combine(TempDir, dbName + ".db");
                File.Copy(template, newVault);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
