/* Program.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 17, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      Form1
 * REQUIRED BY:   (None)
 * 
 * The Cryptnos launching application.  See the Cryptnos form code for more details.
 * 
 * This program is Copyright 2009, Jeffrey T. Darlington.
 * E-mail:  jeff@gpf-comics.com
 * Web:     http://www.gpf-comics.com/
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of
 * the GNU General Public License as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See theGNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program;
 * if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA  02110-1301, USA.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Win32;

namespace com.gpfcomics.Cryptnos
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Just in case something blows up, put on our asbestos underpants:
            try
            {
                // Try to open the Cryptnos registry key:
                RegistryKey CryptnosSettings = Registry.CurrentUser.OpenSubKey("Software",
                    false).OpenSubKey("GPF Comics", false).OpenSubKey("Cryptnos", false);
                if (CryptnosSettings != null)
                {
                    // Grab the version number of the last run version of Cryptnos from the
                    // registry and close the key:
                    Version regVersion =
                        new Version((string)CryptnosSettings.GetValue("Version", "0.0.0.0"));
                    CryptnosSettings.Close();
                    // Compare the last-run version to the version number of the currently
                    // running version.  If the last-run version is newer, show a warning to
                    // the user, giving them an opportunity to exit without clobbering the
                    // settings of the newer version.
                    if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(regVersion)
                        < 0 && MessageBox.Show("It seems that you have previously run a newer " +
                        "version  of Cryptnos (" + regVersion.ToString() + ") than the version " +
                        "you are now running (" +
                        Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                        ").  Please note that newer versions may introduce incompatibilities " +
                        "that may disrupt how older versions work.  Would you like to " +
                        "close this copy of Cryptnos now, just in case?  Note that if you " +
                        "chose to continue, you may loose settings from your newer version.",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                        DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    // If they don't care about their old settings, or if the current version
                    // is the same or newer, go ahead and star the app:
                    else StartIt();
                }
                // If we couldn't get the settings, then we're probably running Cryptnos for
                // the very first time.  Go for it!
                else StartIt();
            }
            // If anything blew up, warn the user and just exit:
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// This method runs the usual .NET Windows app initialization, but that has been
        /// abstracted here so we can potentially call it from multiple locations, promoting
        /// code reuse and simplification.
        /// </summary>
        private static void StartIt()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}