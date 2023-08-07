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
        /// Inserts new user in the database.
        /// </summary>
        /// <param name="user"></param>
        public static void Insert(string un, string pw)
        {
            var con = new SQLiteConnection(ConnectionString());
            try
            {
                con.Open();
                var cmd = new SQLiteCommand(con)
                {
                    CommandText = "INSERT INTO User (Username, Password)" +
                                  "VALUES (@Username,@Password)"
                };
                cmd.Prepare();
                cmd.Parameters.AddWithValue(@"Username", Hash.ToSHA_256(un));
                cmd.Parameters.AddWithValue(@"Password", Hash.ToSHA_256(pw));
                cmd.ExecuteNonQuery();
                MessageBox.Show("Your new account has been created!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { con.Close(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="un"></param>
        /// <param name="pw"></param>
        private static bool VerifiedUser(string un, string pw)
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
                cmd.Parameters.AddWithValue(@"Username", Hash.ToSHA_256(un));  
                cmd.Parameters.AddWithValue(@"Password", Hash.ToSHA_256(pw));

                int res = Convert.ToInt32(cmd.ExecuteScalar());
                return res == 1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return false; }
            finally { con.Close(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="un"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        public static int Uid(string un, string pw)
        {
            if (VerifiedUser(un, pw))
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
                    cmd.Parameters.AddWithValue(@"Username", Hash.ToSHA_256(un));
                    cmd.Parameters.AddWithValue(@"Password", Hash.ToSHA_256(pw));
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
