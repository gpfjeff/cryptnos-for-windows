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
 * UPDATES FOR 1.2.0:  Fixed a bug in the initial version check which blew up on certain systems
 * if the registry keys did not exist.
 * 
 * UPDATES FOR 1.2.2:  Added code to prevent multiple instances of Cryptnos from running at the
 * same time.
 * 
 * UPDATES FOR 1.3.3:  Fixed mutex bug; see Issue #8 in the Google Code issue tracker.
 * 
 * This program is Copyright 2012, Jeffrey T. Darlington.
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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Win32;

namespace com.gpfcomics.Cryptnos
{
    static class Program
    {
        /// <summary>
        /// Set the foreground window using the specified window pointer.  This is primarily
        /// used to redirect the user to the currently open Cryptnos window if they try to
        /// run more than one instance of the program at the same time.
        /// </summary>
        /// <param name="hWnd">A pointer to another Cryptnos instance</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// This <see cref="Mutex"/> will help us ensure that only one instance of Cryptnos
        /// can run at any given time.
        /// </summary>
        static Mutex oneCopyMutex = null;

        /// <summary>
        /// This <see cref="Guid"/> will be used in the generation of our <see cref="Mutex"/>
        /// to ensure that only one instance of Cryptnos can run at any given time.
        /// </summary>
        static Guid mutexGuid = new Guid("492554CF-63EC-44BE-90AD-600CF6F9420A");

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
                        "version of Cryptnos (" + regVersion.ToString() + ") than the version " +
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
            // Originally, if anything blew up above, this just put up an error message and
            // exited.  Unfortunately, this seemed to cause some bizarre and difficult to trace
            // errors, the worst of which was a dreaded "Object reference not set to an
            // instance of an object" that offered no hope of debugging.  This only seemed to
            // crop up in unpredictable occasions and baffled me for months.  Unfortunately,
            // here's where the problem lay:  If the app was running for the very first time,
            // some systems (especially Windows 7) would blow up trying to open the registry
            // keys above which did not exist (obviously since this was the first time the
            // program ran).  So if the above blows up, just try to run the program anyway.
            // The main form has redundant checks here to handle similiar situations, but
            // the check here just didn't have it.  See Issue #1 in the Google Code issue
            // tracker for more details.
            catch { StartIt(); }

        }

        /// <summary>
        /// This method runs the usual .NET Windows app initialization, but that has been
        /// abstracted here so we can potentially call it from multiple locations, promoting
        /// code reuse and simplification.
        /// </summary>
        private static void StartIt()
        {
            // The following single-instance test code was adapted from the following URLs:
            // http://iridescence.no/post/CreatingaSingleInstanceApplicationinC.aspx
            // http://kristofverbiest.blogspot.com/2008/11/creating-single-instance-application.html
            //
            // Create a Boolean flag which we'll test to see if we've created a new window
            // or not.  By default, assume that we've created an entirely new window.
            bool createdNew = true;
            // Asbestos underpants:
            try
            {
                // Create a mutex which will control our state.  Take particular note of the mutex name.
                // First, it starts with "Local\"; if it did not, it would create a global mutex which
                // would require administrative privileges, something not every user may necessarily have.
                // I believe this has been causing issues for certain users with tight security policies.
                // In this case "Local\" applies to local for the user.  Next, we include the application
                // name, both to make it unique to us and easier to identify.  Lastly, we add in a GUID
                // to avoid potential collisions with other apps.
                using (oneCopyMutex = new Mutex(true, "Local\\Cryptnos{" + mutexGuid.ToString() + "}", out createdNew))
                {
                    // Now let's test our new window flag.  If the mutex was newly created, we're
                    // safe to start the new window:
                    if (createdNew)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new Form1());
                    }
                    // Otherwise, the mutex exists and we should already have a window open.
                    // We'll try to redirect the user to that window rather than open a new one.
                    else
                    {
                        // Declare another flag to see whether or not we found the other
                        // instance.  Be default, assume we haven't:
                        bool foundIt = false;
                        // Get the current process, then try to find any currently running
                        // processes by the same name:
                        Process current = Process.GetCurrentProcess();
                        foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                        {
                            // If we found one and the process ID numbers do not match, try to
                            // bring the other instance to the foreground:
                            if (process.Id != current.Id)
                            {
                                foundIt = true;
                                SetForegroundWindow(process.MainWindowHandle);
                                break;
                            }
                        }
                        // Did we find the other instance?  If not, warn the user:
                        if (!foundIt)
                            MessageBox.Show("Windows reports that another instance of Cryptnos " +
                                "is currently running, but we can't find it. You may need to " +
                                "manually stop the other Cryptnos process or reboot your " +
                                "computer before you can run Cryptnos again.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                    } // else

                    // Release the mutex so other copies can run now:
                    oneCopyMutex.ReleaseMutex();
                } // using
            }
            // I'm not sure what to do here for the catch block; if something blows up, we don't
            // want the application, but that's covered because it shouldn't get called.  For
            // now, we'll leave this blank.  If something crops up that we need to clean up,
            // this is the place to do it.
            catch { }
        }  // StartIt()
    }
}