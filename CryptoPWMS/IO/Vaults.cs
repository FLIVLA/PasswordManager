using CryptoPWMS.Security;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
        public static readonly string BaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");

        /// <summary>
        /// Creates a new vault instance in the application base directory.
        /// On file creation the DDL command is executed to create necessary
        /// tables for password vault.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="masterPassword"></param>
        public static void Create(string dbName)
        {
            var conStr = $"Data Source={BaseDir}\\{dbName}";

            if (File.Exists(Path.Combine(BaseDir, dbName))) {
                MessageBox.Show("Database of that name already exist.");
                return;
            }

            using (var fs = new FileStream(Path.Combine(BaseDir, dbName), FileMode.Create)) { 
                fs.Close();
            }

            using (var con = new SQLiteConnection(conStr))
            {
                try {
                    con.Open();
                    var cmd = new SQLiteCommand(con) {
                        CommandText =

                            "CREATE TABLE PasswordGroup (" +
                            "Id INTEGER NOT NULL UNIQUE," +
                            "Name TEXT NOT NULL UNIQUE," +
                            "PRIMARY KEY(Id AUTOINCREMENT));" +

                            "CREATE TABLE Password (" +
                            "Id INTEGER NOT NULL UNIQUE," +
                            "Grp_Id INTEGER NOT NULL," +
                            "Platform TEXT NOT NULL," +
                            "URL TEXT," +
                            "Username BLOB NOT NULL," +
                            "Password BLOB NOT NULL," +
                            "Key BLOB NOT NULL," +
                            "Salt BLOB NOT NULL," +
                            "IV BLOB NOT NULL," +
                            "LastUpdated TEXT NOT NULL," +
                            "FOREIGN KEY(Grp_Id) REFERENCES PasswordGroup(Id)," +
                            "PRIMARY KEY(Id AUTOINCREMENT));" +

                            "INSERT INTO PasswordGroup (Name) VALUES " +
                            "('Social Media')," +
                            "('Streaming Services')," +
                            "('Email Accounts')," +
                            "('Work & Productivity')," +
                            "('Other')"
                    };

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    con.Close();
                    return;
                }
                finally { con.Close(); }
            }
        }
    }
}
