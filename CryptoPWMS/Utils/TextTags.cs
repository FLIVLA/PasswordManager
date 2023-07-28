﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Utils
{
    public static class TextTags
    {
        public static string AppTitle()
        { 
            return
                @"╔══════════════════════════════════════════════════════════════════════════════════════════════╗" + Environment.NewLine+
                @"║   ________  ________  ________   ________  ___       __   ________  ________  ________       ║" + Environment.NewLine +
                @"║  |\   __  \|\   __  \|\   ____\ |\   ____\|\  \     |\  \|\   __  \|\   __  \|\   ___ \      ║" + Environment.NewLine +
                @"║  \ \  \|\  \ \  \|\  \ \  \___|_\ \  \___|\ \  \    \ \  \ \  \|\  \ \  \|\  \ \  \_|\ \     ║" + Environment.NewLine +
                @"║   \ \   ____\ \   __  \ \_____  \\ \_____  \ \  \  __\ \  \ \  \\\  \ \   _  _\ \  \ \\ \    ║" + Environment.NewLine +
                @"║    \ \  \___|\ \  \ \  \|____|\  \\|____|\  \ \  \|\  \_\  \ \  \\\  \ \  \\  \\ \  \_\\ \   ║" + Environment.NewLine +
                @"║     \ \__\    \ \__\ \__\____\_\  \ ____\_\  \ \____________\ \_______\ \__\\ _\\ \_______\  ║" + Environment.NewLine +
                @"║      \|__|     \|__|\|__|\_________\\_________\|____________|\|_______|\|__|\|__|\|_______|  ║" + Environment.NewLine +
                @"║                         \|_________\|_________|                                              ║" + Environment.NewLine +
                @"║                   ___       ________  ________  ___  __    _______   ________                ║" + Environment.NewLine +
                @"║                  |\  \     |\   __  \|\   ____\|\  \|\  \ |\  ___ \ |\   __  \               ║" + Environment.NewLine +
                @"║                  \ \  \    \ \  \|\  \ \  \___|\ \  \/  /|\ \   __/|\ \  \|\  \              ║" + Environment.NewLine +
                @"║                   \ \  \    \ \  \\\  \ \  \    \ \   ___  \ \  \_|/_\ \   _  _\             ║" + Environment.NewLine +
                @"║                    \ \  \____\ \  \\\  \ \  \____\ \  \\ \  \ \  \_|\ \ \  \\  \|            ║" + Environment.NewLine +
                @"║                     \ \_______\ \_______\ \_______\ \__\\ \__\ \_______\ \__\\ _\            ║" + Environment.NewLine +
                @"║                      \|_______|\|_______|\|_______|\|__| \|__|\|_______|\|__|\|__|           ║" + Environment.NewLine +
                @"║                                                                                              ║" + Environment.NewLine +
                //@"╠═════════════════════════════════════════════╤════════════════════════════════════════════════╣" + Environment.NewLine +
                //@"║  Secure Password Management System          │  Created by Frederik Lind | flin@itu.dk        ║" + Environment.NewLine +
                //@"║  Current Version: 1.0.0                     │  ITU - AIS Exam Project   | Summer 2023        ║" + Environment.NewLine +
                @"╚══════════════════════════════════════════════════════════════════════════════════════════════╝";

        }

        public static string PasswordGenerator()
        {
            return
                @" ________  ________  ________   ________  ___       __   ________  ________  ________          ________  _______   ________   _______   ________  ________  _________  ________  ________     " + Environment.NewLine +
                @"|\   __  \|\   __  \|\   ____\ |\   ____\|\  \     |\  \|\   __  \|\   __  \|\   ___ \        |\   ____\|\  ___ \ |\   ___  \|\  ___ \ |\   __  \|\   __  \|\___   ___\\   __  \|\   __  \    " + Environment.NewLine +
                @"\ \  \|\  \ \  \|\  \ \  \___|_\ \  \___|\ \  \    \ \  \ \  \|\  \ \  \|\  \ \  \_|\ \       \ \  \___|\ \   __/|\ \  \\ \  \ \   __/|\ \  \|\  \ \  \|\  \|___ \  \_\ \  \|\  \ \  \|\  \   " + Environment.NewLine +
                @" \ \   ____\ \   __  \ \_____  \\ \_____  \ \  \  __\ \  \ \  \\\  \ \   _  _\ \  \ \\ \       \ \  \  __\ \  \_|/_\ \  \\ \  \ \  \_|/_\ \   _  _\ \   __  \   \ \  \ \ \  \\\  \ \   _  _\  " + Environment.NewLine +
                @"  \ \  \___|\ \  \ \  \|____|\  \\|____|\  \ \  \|\__\_\  \ \  \\\  \ \  \\  \\ \  \_\\ \       \ \  \|\  \ \  \_|\ \ \  \\ \  \ \  \_|\ \ \  \\  \\ \  \ \  \   \ \  \ \ \  \\\  \ \  \\  \| " + Environment.NewLine +
                @"   \ \__\    \ \__\ \__\____\_\  \ ____\_\  \ \____________\ \_______\ \__\\ _\\ \_______\       \ \_______\ \_______\ \__\\ \__\ \_______\ \__\\ _\\ \__\ \__\   \ \__\ \ \_______\ \__\\ _\ " + Environment.NewLine +
                @"    \|__|     \|__|\|__|\_________\\_________\|____________|\|_______|\|__|\|__|\|_______|        \|_______|\|_______|\|__| \|__|\|_______|\|__|\|__|\|__|\|__|    \|__|  \|_______|\|__|\|__|" + Environment.NewLine +
                @"                       \|_________\|_________|                                                                                                                                                ";

        }
    }
}