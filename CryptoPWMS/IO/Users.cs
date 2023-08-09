using CryptoPWMS.Security;
using System;
using System.Data.SQLite;
using System.Windows;

namespace CryptoPWMS.IO
{
    /*
     *    ____ ___                           
     *    |    |   \______ ___________  ______
     *    |    |   /  ___// __ \_  __ \/  ___/
     *    |    |  /\___ \\  ___/|  | \/\___ \ 
     *    |______//____  >\___  >__|  /____  >
     *                 \/     \/           \/ 
     */

    /// <summary>
    /// 
    /// </summary>
    internal class Users
    {
        /// <summary>
        /// Returns connection string of the database in the current executing directory.
        /// </summary>
        /// <returns>The connection string for the application database.</returns>
        private static string ConnectionString()
        {
            return $"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\Database\\pwdb.db;";
        }

        /// <summary>
        /// Inserts new user in the database. Takes username and password
        /// and computes SHA256 values of the plaintext values for secure storage,
        /// of the username/password combination in the database.
        /// </summary>
        /// <param name="username">Username of the new user account (plaintext).</param>
        /// <param name="password">Password of the new user account (plaintext).</param>
        public static void Insert(string username, string password)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();                                                           // Opens the database connection.
                var cmd = new SQLiteCommand(con)                                      // initializes new SQLite command with the specified connection.
                {
                    CommandText = "INSERT INTO User (Username, Password, Salt)" +     // creates command with parameterized commandtext.
                                  "VALUES (@Username,@Password, @Salt)"
                };

                byte[] salt = Crypto.GenerateSalt();

                cmd.Prepare();                                                  
                cmd.Parameters.AddWithValue(@"Username", (username));                           // Sets parameter value of 'Username' to input value.
                cmd.Parameters.AddWithValue(@"Password", Crypto.Hash_Argon2(password, salt));   // Sets parameter value of 'Password' to a hashed representation of the input value.
                cmd.Parameters.AddWithValue(@"Salt", salt);
                cmd.ExecuteNonQuery();                                                          // Executes the command on the database.
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }                                                            // Ensures that the database connection is closed.
        }

        /// <summary>
        /// Verifies that a user of the specified username/password combination
        /// exist in the database, by comparing a hashed value representation
        /// of the plaintext input values with the stored hashed values of the database.
        /// As the username column of the User table are set to conatin unique values,
        /// the result of the count query should return 1 if any match is found.
        /// </summary>
        /// <param name="username">Username of the account to be verified (plaintext).</param>
        /// <param name="password">Password if the account to be verified (plaintext).</param>
        public static string GetUsername(string username, string password)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var cmd = new SQLiteCommand(con)
                {
                    CommandText = "SELECT Username, Password, Salt FROM User " +
                                  "WHERE Username=@Username"
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue(@"Username", username);  

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hash = reader.GetString(1);
                        byte[] salt = (byte[])reader["Salt"];
                        App.Salt = salt;
                        string inputHash = Crypto.Hash_Argon2(password, salt);       // Hash input password with Argon2 using the stored salt.
                        if (hash.Equals(inputHash)) { 
                            return Crypto.Hash_Argon2(reader.GetString(0), salt);   // return username if hash matches hash of input password hashed with stored salt.
                        }   
                    }
                    else return "";                                                  // If no match of the input username was found in the database.
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return ""; }
            finally { con.Close(); }
            return "";
        }
    }
}
