using CryptoPWMS.Components;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace CryptoPWMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static int _cur_uid = -1;
        public static int Cur_Uid { get => _cur_uid; set => _cur_uid = value; } 

        #region UI_ELEMENTS
        
        private static MainWindow? mainWin;
        private static HomeScreen? homeScreen;
        private static MainUI? mainUI;

        public static MainWindow? MainWin { get => mainWin; set => mainWin = value; }
        public static HomeScreen? HomeScreen { get => homeScreen; set => homeScreen = value; }
        public static MainUI? MainUI { get => mainUI; set => mainUI = value; }

        #endregion
    }
}
