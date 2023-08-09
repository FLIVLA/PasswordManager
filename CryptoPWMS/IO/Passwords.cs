using CryptoPWMS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Windows;
using CryptoPWMS.Security;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Policy;

namespace CryptoPWMS.IO
{
    /*
     *   __________                                              .___      
     *   \______   \_____    ______ ________ _  _____________  __| _/______
     *   |     ___/\__  \  /  ___//  ___/\ \/ \/ /  _ \_  __ \/ __ |/  ___/
     *   |    |     / __ \_\___ \ \___ \  \     (  <_> )  | \/ /_/ |\___ \ 
     *   |____|    (____  /____  >____  >  \/\_/ \____/|__|  \____ /____  >
     *                  \/     \/     \/                          \/    \/ 
     */

    /// <summary>
    /// Helper class for all password operations on the current user's
    /// personal password vault.
    /// </summary>
    internal static class Passwords
    {
        /// <summary>
        /// Returns connection string of the database in the current executing directory.
        /// </summary>
        /// <returns>The connection string of the database.</returns>
        private static string ConnectionString()
        {
            return $"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\Database\\{App.Cur_User}.db;";
        }

        #region ============================ GET ============================

        /// <summary>
        /// Gets all password groups from the database. These are 
        /// non-user-specific records, and will return the same default 
        /// groups regardless of the active user ID.
        /// </summary>
        /// <returns>All password groups</returns>
        public static List<PasswordGroup> Get_PWGroups()
        {
            using (IDbConnection con = new SQLiteConnection(ConnectionString()))
            {
                var groups = con.Query<PasswordGroup>("SELECT * FROM PasswordGroup", new DynamicParameters());
                return groups.ToList();
            }
        }

        /// <summary>
        /// Gets all password records matching the current active user ID,
        /// from the database.
        /// </summary>
        /// <returns>All Password records from the user's vault.</returns>
        public static List<PasswordItem> GetAll()
        {
            var pws = new List<PasswordItem>();
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var query = "SELECT * FROM Password";

                pws = con.Query<PasswordItem>(query).OrderBy(x => x.Platform).ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }
            return pws;
        }

        /// <summary>
        /// Gets all passwords matching the foreign key value for
        /// Password group ID passed in the method arguments, and
        /// current active user id.
        /// </summary>
        /// <param name="grp">Group ID for the passwords.</param>
        /// <returns></returns>
        public static List<PasswordItem> GetByGroup(int grp)
        {
            var pws = new List<PasswordItem>();                         // Initialize new empty list of type PasswordItem
            var con = new SQLiteConnection(ConnectionString());         
            try
            {
                con.Open();                                             // Open the database connection.
                var query = "SELECT * FROM Password " +
                            "WHERE Grp_Id=@Grp_Id";

                pws = con.Query<PasswordItem>(query, new { Grp_Id = grp }).OrderBy(x => x.Platform).ToList();
                string s = "";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }                                    // Ensures that database connection is closed.
            return pws;
        }

        #endregion

        #region =========================== INSERT ===========================

        /// <summary>
        /// Inserts new password record in the database. 
        /// Generates a unique random encryption key and salt and initialization vector 
        /// for the password entry, these are then used to encrypt the plaintext values
        /// of the entered username and password. The unique salt and key is then 
        /// encrypted by the master key and salt for safe storage in the database. 
        /// </summary>
        /// <param name="grp">Group ID for the password entry (Foreign key).</param>
        /// <param name="platform">Platform name for the password entry.</param>
        /// <param name="url">URL for the password entry, pass empty string if not specified.</param>
        /// <param name="username">Username value in plaintext.</param>
        /// <param name="password">Password value in plaintext.</param>
        public static void Insert(int grp, string platform, string url, string username, string password)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                byte[] key = Crypto.GenerateKey();                                                      // Generate unique encryption key.             
                byte[] salt = Crypto.GenerateSalt();                                                    // Generate unique salt.
                byte[] iv;                                                                              // Initialize variable to store initialization vector for AES.

                using (Aes aes = Aes.Create())
                {
                    aes.GenerateIV();                                                                   // Generate unique IV.
                    iv = aes.IV;                                                                        // store IV.

                    byte[] crypto_un = Crypto.Encrypt_AES(Encoding.UTF8.GetBytes(username), key, salt, iv);     // Encrypt username
                    byte[] crypto_pw = Crypto.Encrypt_AES(Encoding.UTF8.GetBytes(password), key, salt, iv);     // Encrypt password

                    byte[] encryptedKey = Crypto.Encrypt_AES(key, App.DerivedKey, App.Salt, iv);
                    byte[] encryptedSalt = Crypto.Encrypt_AES(salt, App.DerivedKey, App.Salt, iv);

                    con.Open();
                    var cmd = new SQLiteCommand(con)
                    {
                        CommandText = "INSERT INTO Password (Grp_Id, Platform, URL, Username, Password, Key, Salt, IV, LastUpdated)" +
                                      "VALUES(@Grp_Id, @Platform, @URL, @Username, @Password, @Key, @Salt, @IV, @LastUpdated)"
                    };
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue(@"Grp_Id", grp);
                    cmd.Parameters.AddWithValue(@"Platform", platform);
                    cmd.Parameters.AddWithValue(@"URL", url);
                    cmd.Parameters.AddWithValue(@"Username", crypto_un);
                    cmd.Parameters.AddWithValue(@"Password", crypto_pw);
                    cmd.Parameters.AddWithValue(@"Key", encryptedKey);
                    cmd.Parameters.AddWithValue(@"Salt", encryptedSalt);
                    cmd.Parameters.AddWithValue(@"IV", iv);
                    cmd.Parameters.AddWithValue(@"LastUpdated", DateTime.Now.ToString("MM/dd/yyyy HH:mm"));

                    cmd.ExecuteNonQuery();
                }                                 
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }                                        // Ensures that database connection is closed.                                                               
        }

        /// <summary>
        /// Inserts new group into the database, enables the user to add custom folders
        /// in the password vault.
        /// </summary>
        /// <param name="grpName"></param>
        public static void InsertGroup(string grpName)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var cmd = new SQLiteCommand(con)
                {
                    CommandText = "INSERT INTO PasswordGroup (Name) VALUES(@Name)"
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Name", grpName);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }
        }

        #endregion

        #region ====================== UPDATE/DELETE ======================

        /// <summary>
        /// Updates a record with primary key ID matching the ID of the
        /// PasswordItem instance passed in the argument. The field values
        /// of the Password item instance should be updated before calling
        /// this method, otherwise changes wont be committed to the database.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void Update(PasswordItem p, string username, string password)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                byte[] key = Crypto.GenerateKey();                                                      // Generate unique encryption key.             
                byte[] salt = Crypto.GenerateSalt();                                                    // Generate unique salt.
                byte[] iv;                                                                              // Initialize variable to store initialization vector for AES.

                using (var aes = Aes.Create())
                {
                    aes.GenerateIV();
                    iv = aes.IV;

                    byte[] crypto_un = Crypto.Encrypt_AES(Encoding.UTF8.GetBytes(username), key, salt, iv);     // Encrypt username
                    byte[] crypto_pw = Crypto.Encrypt_AES(Encoding.UTF8.GetBytes(password), key, salt, iv);     // Encrypt password

                    byte[] encryptedKey = Crypto.Encrypt_AES(key, App.DerivedKey, App.Salt, iv);
                    byte[] encryptedSalt = Crypto.Encrypt_AES(salt, App.DerivedKey, App.Salt, iv);

                    con.Open();
                    var cmd = new SQLiteCommand(con)
                    {
                        CommandText = "UPDATE Password SET " +
                                      "Grp_Id=@Grp_Id, " +
                                      "Platform=@Platform, " +
                                      "URL=@URL, " +
                                      "Username=@Username, " +
                                      "Password=@Password, " +
                                      "Key=@Key, " +
                                      "Salt=@Salt, " +
                                      "IV=@IV, " +
                                      "LastUpdated=@LastUpdated WHERE Id=@Id"
                    };
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue(@"Grp_Id", p.Grp_Id);
                    cmd.Parameters.AddWithValue(@"Platform", p.Platform);
                    cmd.Parameters.AddWithValue(@"URL", p.URL);
                    cmd.Parameters.AddWithValue(@"Username", crypto_un);
                    cmd.Parameters.AddWithValue(@"Password", crypto_pw);
                    cmd.Parameters.AddWithValue(@"Key", encryptedKey);
                    cmd.Parameters.AddWithValue(@"Salt", encryptedSalt);
                    cmd.Parameters.AddWithValue(@"IV", iv);
                    cmd.Parameters.AddWithValue(@"LastUpdated", DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                    cmd.Parameters.AddWithValue(@"Id", p.Id);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }
        }

        /// <summary>
        /// Deletes a password record matching the Id of the PasswordItem
        /// instance passed in the mthod arguments, from the database.
        /// </summary>
        /// <param name="p">The password item instance to be deleted from the database</param>
        public static void Delete(PasswordItem p)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var cmd = new SQLiteCommand(con)                         // initialize new SQLiteCommand with the SQLiteConnection
                {
                    CommandText = "DELETE FROM Password WHERE Id=@Id"    // Set commandstring with parameters.
                };
                cmd.Parameters.AddWithValue("@Id", p.Id);                // Add query parameters.
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }        // Shows exception message in msgbx.
            finally { con.Close(); }                                     // Ensures that database connection is closed.
        }

        #endregion
    }
}
