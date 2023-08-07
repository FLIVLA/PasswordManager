using CryptoPWMS.Security;
using System;
using System.Data.SQLite;
using System.Windows;

namespace CryptoPWMS.IO
{
    internal class Users
    {
        /// <summary>
        /// Returns connection string of the database in the current executing directory.
        /// </summary>
        /// <returns></returns>
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
                con.Open();                                                     // Opens the database connection.
                var cmd = new SQLiteCommand(con)                                // initializes new SQLite command with the specified connection.
                {
                    CommandText = "INSERT INTO User (Username, Password)" +     // creates command with parameterized commandtext.
                                  "VALUES (@Username,@Password)"
                };
                cmd.Prepare();                                                  
                cmd.Parameters.AddWithValue(@"Username", Hash.ToSHA_256(username));   // Sets parameter value of 'Username' to a hashed representation of the input value.
                cmd.Parameters.AddWithValue(@"Password", Hash.ToSHA_256(password));   // Sets parameter value of 'Password' to a hashed representation of the input value.
                cmd.ExecuteNonQuery();                                                // Executes the command on the database.
                MessageBox.Show("Your new account has been created!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }                                            // Ensures that the database connection is closed.
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
        private static bool VerifiedUser(string username, string password)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var cmd = new SQLiteCommand(con)
                {
                    CommandText = "SELECT COUNT(1) FROM User " +
                                  "WHERE Username=@Username AND Password=@Password"
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue(@"Username", Hash.ToSHA_256(username));  
                cmd.Parameters.AddWithValue(@"Password", Hash.ToSHA_256(password));

                int res = Convert.ToInt32(cmd.ExecuteScalar());
                return res == 1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return false; }
            finally { con.Close(); }
        }

        /// <summary>
        /// If user is passes the verified function, the user ID will be returned,
        /// of the user record matching the hashed value representation of 
        /// plaintext inputs of username and password. 
        /// </summary>
        /// <param name="username">Username of the account to be verified and retrieve ID (plaintext).</param>
        /// <param name="password">Password of the account to be verified and retrieve ID (plaintext).</param>
        /// <returns></returns>
        public static int Uid(string username, string password)
        {
            if (VerifiedUser(username, password))
            {
                var con = new SQLiteConnection(ConnectionString());
                try
                {
                    con.Open();
                    var cmd = new SQLiteCommand(con)
                    {
                        CommandText = "SELECT Id FROM User " +
                                      "WHERE Username=@Username AND Password=@Password"
                    };
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue(@"Username", Hash.ToSHA_256(username));
                    cmd.Parameters.AddWithValue(@"Password", Hash.ToSHA_256(password));
                    int res = Convert.ToInt32(cmd.ExecuteScalar());
                    return res;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); return -1; }
                finally { con.Close(); }
            }
            return -1;
        }
    }
}
