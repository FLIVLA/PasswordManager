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

namespace CryptoPWMS.IO
{
    internal static class Passwords
    {
        /// <summary>
        /// Returns connection string of the database in the current executing directory.
        /// </summary>
        /// <returns>The connection string of the database.</returns>
        private static string ConnectionString()
        {
            return $"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\Database\\pwdb.db;";
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
        public static List<PasswordItem> GetByUserId(int uid)
        {
            var pws = new List<PasswordItem>();
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var query = "SELECT * FROM Password " +
                            "WHERE User_Id=@User_Id";

                pws = con.Query<PasswordItem>(query, new { User_Id = uid }).ToList();
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
                            "WHERE User_Id=@User_Id AND Grp_Id=@Grp_Id";

                pws = con.Query<PasswordItem>(query, new { 
                    User_Id = App.Cur_Uid, Grp_Id = grpid
                }).ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }                                    // Ensures that database connection is closed.
            return pws;
        }

        #endregion

        #region ========================== INSERT ==========================

        /// <summary>
        /// Inserts new password record in the database.
        /// </summary>
        /// <param name="uid">User ID of the current active user.</param>
        /// <param name="grp">Group ID of the password group associated with the new entry</param>
        /// <param name="platform">Name of the platform of the new entry</param>
        /// <param name="username">Username of the new entry</param>
        /// <param name="password">Password of the new entry</param>
        public static void Insert(int uid, int grp, string platform, string url, string username, string password)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                byte[] key = Crypto.GenerateKey();                                                  // Generate unique encryption key.
                byte[] salt = Crypto.GenerateSalt();                                                // Generate unique salt.
                byte[] crypto_un = Crypto.Encrypt(Encoding.UTF8.GetBytes(username), key, salt);     // Encrypt username
                byte[] crypto_pw = Crypto.Encrypt(Encoding.UTF8.GetBytes(password), key, salt);     // Encrypt password

                con.Open();
                var cmd = new SQLiteCommand(con)
                {
                    CommandText = "INSERT INTO Password (User_Id, Grp_Id, Platform, URL, Username, Password, LastUpdated)" +
                                  "VALUES(@User_Id, @Grp_Id, @Platform, @URL, @Username, @Password, @LastUpdated)"
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue(@"User_Id", uid);
                cmd.Parameters.AddWithValue(@"Grp_Id", grp);
                cmd.Parameters.AddWithValue(@"Platform", platform);
                cmd.Parameters.AddWithValue(@"URL", url);
                cmd.Parameters.AddWithValue(@"Username", username);
                cmd.Parameters.AddWithValue(@"Password", password);
                cmd.Parameters.AddWithValue(@"", key);
                cmd.Parameters.AddWithValue(@"", salt);
                cmd.Parameters.AddWithValue(@"LastUpdated", DateTime.Now.ToString("MM/dd/yyyy HH:mm"));

                cmd.ExecuteNonQuery();                                  
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }                                        // Ensures that database connection is closed.                                                               
        }

        #endregion

        #region ======================== UPDATE/DELETE ========================

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
                    CommandText = "DELETE FROM Password WHERE Id=@Id AND User_Id=@User_Id"      // Set commandstring with parameters.
                };
                cmd.Parameters.AddWithValue("@Id", p.Id);                                       //
                cmd.Parameters.AddWithValue("@User_Id", App.Cur_Uid);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }                               // Shows exception message in msgbx.
            finally { con.Close(); }                                                            // Ensures that database connection is closed.
        }

        #endregion
    }
}
