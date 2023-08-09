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
    /// 
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
        /// <param name="uid">Current active user ID.</param>
        /// <returns>All Password records matching the user ID</returns>
        public static List<PasswordItem> GetAll()
        {
            var pws = new List<PasswordItem>();
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var query = "SELECT * FROM Password";

                pws = con.Query<PasswordItem>(query).ToList();
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
        /// <param name="uid">User ID of the current active user.</param>
        /// <param name="grpid"></param>
        /// <returns></returns>
        public static List<PasswordItem> GetByGroup(int grpid)
        {
            var pws = new List<PasswordItem>();                         // Initialize new empty list of type PasswordItem
            var con = new SQLiteConnection(ConnectionString());         
            try
            {
                con.Open();                                             // Open the database connection.
                var query = "SELECT * FROM Password " +
                            "WHERE Grp_Id=@Grp_Id";

                pws = con.Query<PasswordItem>(query, new { Grp_Id = grpid }).ToList();
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
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="platform"></param>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
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

        #endregion

        #region ====================== UPDATE/DELETE ======================

        /// <summary>
        /// Updates a record with primary key ID matching the ID of the
        /// PasswordItem instance passed in the argument. The field values
        /// of the Password item instance should be updated before calling
        /// this method, otherwise changes wont be committed to the database.
        /// </summary>
        /// <param name="p">Updated instance to be committed to database.</param>
        public static void Update(PasswordItem p)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {

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
                var cmd = new SQLiteCommand(con)                                                // initialize new SQLiteCommand with the SQLiteConnection
                {
                    CommandText = "DELETE FROM Password WHERE Id=@Id"                           // Set commandstring with parameters.
                };
                cmd.Parameters.AddWithValue("@Id", p.Id);                                       //
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }                               // Shows exception message in msgbx.
            finally { con.Close(); }                                                            // Ensures that database connection is closed.
        }

        #endregion
    }
}
