using CryptoPWMS.Components;
using System.IO;
using System.Windows;

namespace CryptoPWMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string _cur_uid = "";
        private static byte[] _derivedKey;
        private static bool _authenticated = false;
        public static string Cur_User { get => _cur_uid; set => _cur_uid = value; }
        public static byte[] DerivedKey { get => _derivedKey; set => _derivedKey = value; }
        public static bool IsAuthenticated { get => _authenticated; set => _authenticated = value; }
        public static byte[] Salt { get; set; }

        #region UI_ELEMENTS
        
        private static MainWindow? mainWin;
        private static HomeScreen? homeScreen;
        private static MainUI? mainUI;

        public static MainWindow? MainWin { get => mainWin; set => mainWin = value; }
        public static HomeScreen? HomeScreen { get => homeScreen; set => homeScreen = value; }
        public static MainUI? MainUI { get => mainUI; set => mainUI = value; }

        #endregion

        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException) { return true; }
        }
    }
}
