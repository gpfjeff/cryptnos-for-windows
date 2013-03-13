/* AdvancedSettingsDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          October 25, 2010
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      Form1
 * REQUIRED BY:   (None)
 * 
 * The Advanced Settings dialog provides an interface for the user to tweak some of the more
 * advanced settings behind the scenes of Cryptnos.  These settings are passed in from the
 * main application and the results are made available as read-only properties that the app
 * can pull back out.
 * 
 * Initially, the only advanced setting available is the text encoding used when generating
 * passwords.  The encoding used for storage should *ALWAYS* be UTF-8.  Users will be strongly
 * encouraged to use UTF-8 for their passwords for the greatest cross-platform compatibility.
 * 
 * UPDATES FOR 1.2.1:  Added checkboxes for debug mode and disabling the update check.  It's
 * silly to have "hidden" settings with no UI other than tweaking the registry, so we might
 * as well add them, and the advanced settings would definitely be the place to put them.
 * Also added an option to force an update check at next launch (disabled if update checks are
 * disabled).
 * 
 * UPDATES FOR 1.3.1:  Added Show Master Passphrase and Clear Passwords on Focus Loss
 * checkboxes.
 * 
 * UPDATES FOR 1.3.3:  Made UTF-8 the default encoding except for when displaying the system
 * default.  Removed the "force update check on next launch" checkbox and replaced it with a
 * new Check for Updates button that lets the user interactively initiate the check.  Pressing
 * F1 now launches the HTML help.
 * 
 * This program is Copyright 2013, Jeffrey T. Darlington.
 * E-mail:  jeff@cryptnos.com
 * Web:     http://www.cryptnos.com/
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using com.gpfcomics.UpdateChecker;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// This dialog allows the user to tweak advanced settings within Cryptnos, such as
    /// which text encoding to use during password generation.
    /// </summary>
    public partial class AdvancedSettingsDialog : Form, IUpdateCheckListener
    {
        /// <summary>
        /// This delegate is used to reset the Check for Updates button to its default state
        /// in a thread-safe way.  See ResetUpdateCheckButton() for details.
        /// </summary>
        /// <param name="text">The text to set for the button</param>
        private delegate void SetTextCallback(string text);

        /// <summary>
        /// Our parent, the main Cryptnos form
        /// </summary>
        private MainForm parent = null;

        /// <summary>
        /// The currently selected text encoding
        /// </summary>
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Whether or not debug mode is enabled
        /// </summary>
        private bool debug = false;

        /// <summary>
        /// Whether or not to disable the update check
        /// </summary>
        private bool disableUpdateCheck = false;

        /// <summary>
        /// Whether or not Cryptnos should stay on top of other windows
        /// </summary>
        private bool keepOnTop = false;

        /// <summary>
        /// Whether or not Cryptnos should show or obscure the master passphrase
        /// </summary>
        private bool showMasterPassword = false;

        /// <summary>
        /// Whether or not Cryptnos should clear the master passphrase and generated
        /// password when the main form loses focus
        /// </summary>
        private bool clearPasswordsOnFocusLoss = false;

        /// <summary>
        /// Flag to determine whether or not we should show the disable update check warning.
        /// This should be initially true and should be shown at least once when the disable
        /// update check box is checked.
        /// </summary>
        private bool showDisableUpdateCheckWarning = true;

        /// <summary>
        /// A <see cref="Uri"/> for the official Cryptnos updates feed.  The
        /// <see cref="UpdateChecker"/> will use this feed to look for updated versions of
        /// Cryptnos.
        /// </summary>
        private Uri updateFeedUri = new Uri(Properties.Resources.UpdateFeedUri);

        /// <summary>
        /// The unique app string for <see cref="UpdateChecker"/> lookups
        /// </summary>
        private string updateFeedAppName = Properties.Resources.UpdateFeedAppName;

        /// <summary>
        /// The number of days between update checks, which we will passs to the
        /// <see cref="UpdateChecker"/>.
        /// </summary>
        private int updateInterval = Int32.Parse(Properties.Resources.UpdateIntervalInDays);

        /// <summary>
        /// The actual <see cref="UpdateChecker"/> object, which will check for Cryptnos
        /// updates
        /// </summary>
        private UpdateChecker.UpdateChecker updateChecker = null;

        /// <summary>
        /// This string contains the default button text for the Check for Updates button
        /// </summary>
        private string btnCheckForUpdatesText = "Check for Updates...";

        /// <summary>
        /// The currently selected text encoding
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
        }

        /// <summary>
        /// Whether or not debug mode is enabled
        /// </summary>
        public bool Debug
        {
            get { return debug; }
        }

        /// <summary>
        /// Whether or not to disable the update check
        /// </summary>
        public bool DisableUpdateCheck
        {
            get { return disableUpdateCheck; }
        }

        /// <summary>
        /// Whether or not Cryptnos should stay on top of other windows
        /// </summary>
        public bool KeepOnTop
        {
            get { return keepOnTop; }
        }

        /// <summary>
        /// Whether or not Cryptnos should show or obscure the master passphrase
        /// </summary>
        public bool ShowMasterPassword
        {
            get { return showMasterPassword; }
        }

        /// <summary>
        /// Whether or not Cryptnos should clear the master passphrase and generated
        /// password when the main form loses focus
        /// </summary>
        public bool ClearPasswordsOnFocusLoss
        {
            get { return clearPasswordsOnFocusLoss; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parent">Our parent Cryptnos form</param>
        /// <param name="encoding">The current text encoding setting</param>
        /// <param name="showTooltips">A boolean value specifying whether or not to show
        /// tooltip help.</param>
        /// <param name="debug">Whether or not debug mode is enabled</param>
        /// <param name="disableUpdateCheck">Whether or not to disable the update check</param>
        /// <param name="keepOnTop">Whether or not to keep Cryptnos on top of other windows</param>
        /// <param name="showMasterPassword">Whether or not Cryptnos should show or obscure the master passphrase</param>
        /// <param name="clearPasswordsOnFocusLoss">Whether or not Cryptnos should clear the master passphrase and generated
        /// password when the main form loses focus</param>
        public AdvancedSettingsDialog(MainForm parent, Encoding encoding, bool showTooltips, bool debug,
            bool disableUpdateCheck, bool keepOnTop, bool showMasterPassword,
            bool clearPasswordsOnFocusLoss)
        {
            InitializeComponent();
            // Keep track of our parent form:
            this.parent = parent;
            // Set up the encoding's drop-down box:
            foreach (EncodingInfo encodingInfo in Encoding.GetEncodings())
                cmbTextEncodings.Items.Add(encodingInfo.GetEncoding());
            cmbTextEncodings.DisplayMember = "WebName";
            // Select the text encoding currently selected:
            this.encoding = encoding;
            cmbTextEncodings.SelectedItem = encoding;
            // Show the default encoding for the system, mostly for debugging purposes:
            lblDefaultEncoding.Text += Encoding.Default.WebName;
            // Get the debug and update check settings and check or uncheck the appropriate
            // boxes:
            this.debug = debug;
            this.disableUpdateCheck = disableUpdateCheck;
            this.showMasterPassword = showMasterPassword;
            this.clearPasswordsOnFocusLoss = clearPasswordsOnFocusLoss;
            chkDebug.Checked = debug;
            chkDisableUpdateCheck.Checked = disableUpdateCheck;
            chkShowMasterPassphrase.Checked = showMasterPassword;
            chkClearPasswordOnFocusLoss.Checked = clearPasswordsOnFocusLoss;
            // The "keep on top" setting:
            this.keepOnTop = keepOnTop;
            chkKeepOnTop.Checked = keepOnTop;
            this.TopMost = keepOnTop;
            toolTip1.Active = showTooltips;
        }

        /// <summary>
        /// What to do when the OK button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Return an OK result and set the settings to their selected values:
            DialogResult = DialogResult.OK;
            encoding = (Encoding)cmbTextEncodings.SelectedItem;
            debug = chkDebug.Checked;
            disableUpdateCheck = chkDisableUpdateCheck.Checked;
            keepOnTop = chkKeepOnTop.Checked;
            showMasterPassword = chkShowMasterPassphrase.Checked;
            clearPasswordsOnFocusLoss = chkClearPasswordOnFocusLoss.Checked;
            Hide();
        }

        /// <summary>
        /// What to do when the Cancel button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        /// <summary>
        /// What to do when the Debug checkbox is toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            // Immediately turn on or off debug messages.  Note that this won't take affect
            // for the entire app until the user clicks the OK button.
            debug = chkDebug.Checked;
        }

        /// <summary>
        /// Extra things to do when the Disable Update Check checkbox is toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDisableUpdateCheck_CheckedChanged(object sender, EventArgs e)
        {
            // What we do here depends on whether or not the disable update check box is
            // ticked or not.  We'll cover the checked case first:
            if (chkDisableUpdateCheck.Checked)
            {
                // Since we know the box has been checked and the user has chosen to disable
                // the update check, show a warning the first time this change is made in the
                // session.  We don't want to show this when the box is unchecked; only when
                // it is checked.  We also don't want to pester them if they repeatedly toggle
                // it.
                if (showDisableUpdateCheckWarning)
                {
                    MessageBox.Show("Disabling the automatic update check is not recommended. " +
                        "As a security application, it is important to remain current on the " +
                        "latest updates of Cryptnos to keep your passwords and the services " +
                        "they protect secure. While there are valid reasons to disable this " +
                        "check, we do not recommend it. You can always reverse this setting " +
                        "by returning to the Advanced Settings dialog in the future.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    showDisableUpdateCheckWarning = false;
                }
            }
        }

        /// <summary>
        /// What to do when the Check for Updates button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            // Asbestos underpants:
            try
            {
                // Create a new instance of the update checker.  Note that we create a new one
                // each time to make sure that things like the debug flag gets changed if the
                // checkbox is toggled.  Also note that we'll hard code the last update check date
                // to sometime in the past (we'll subtract twice the update interval just to make
                // sure); this will force the update check to occur regardless of when the auto-
                // mattic check last occurred..
                updateChecker = new UpdateChecker.UpdateChecker(updateFeedUri, updateFeedAppName,
                    Assembly.GetExecutingAssembly().GetName().Version, this,
                    DateTime.Now.AddDays(-(updateInterval * 2)), updateInterval,
                    debug);
                // Set the text of the button to a "Please wait..." message, then disable it so
                // the user can't click it again:
                btnCheckForUpdates.Text = "Please wait...";
                btnCheckForUpdates.Enabled = false;
                // Now initiate the update check:
                updateChecker.CheckForNewVersion();
            }
            // If anything blew up, display an error message, then re-enable the Check for Updates
            // button:
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Update Check Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else MessageBox.Show("An error occurred while trying to perform the " +
                    "update check.  Please try another check later.", "Update Check Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnCheckForUpdates.Text = btnCheckForUpdatesText;
                btnCheckForUpdates.Enabled = true;
            }
        }

        /// <summary>
        /// Reset the Check for Updates button to its default state in a thread-safe
        /// way.  Use this method to reset the button when called from any of the
        /// <see cref="IUpdateCheckListener"/> methods.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        private void ResetUpdateCheckButton(string text)
        {
            // Since changing the button's state is a thread-unsafe operation, we need
            // to use a callback to set it if we're invoking it from the update checker
            // thread.  See:
            // http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k%28EHINVALIDOPERATION.WINFORMS.ILLEGALCROSSTHREADCALL%29&rd=true
            if (btnCheckForUpdates.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ResetUpdateCheckButton);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                btnCheckForUpdates.Text = text;
                btnCheckForUpdates.Enabled = true;
            }
        }

        /// <summary>
        /// Process "hot keys" combinations for the entire form
        /// </summary>
        /// <param name="message"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            // See if we recognize any key combinations:
            switch (keys)
            {
                // F1:  Launch the browser to show the HTML help:
                case Keys.F1:
                    // The HTML help file sits in the same folder as the Cryptnos executable.
                    // So to get the location of the EXE and append the help file name to the
                    // folder's path.
                    FileInfo mainExePath = new FileInfo(Application.ExecutablePath);
                    string helpIndex = mainExePath.DirectoryName +
                        Char.ToString(System.IO.Path.DirectorySeparatorChar) +
                        "help.html";
                    // The file should exist, but just in case it doesn't:
                    if ((new FileInfo(helpIndex)).Exists)
                    {
                        // Try to launch the default browser.  We'll pass the path to the HTML file
                        // to the system and let it handle what browser to open.  Whatever is
                        // associated with HTML files should be launched.  However, just in case
                        // something blows up, we'll include this in a try/catch and display an
                        // error if it fails.
                        try { System.Diagnostics.Process.Start(helpIndex); }
                        catch
                        {
                            MessageBox.Show("I was unable to launch your default browser to display " +
                                "the Cryptnos help file. Please use the shortcut icon in the Start " +
                                "menu instead.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    // If the help files can't be found, complain:
                    else
                    {
                        MessageBox.Show("The Cryptnos HTML help file could not be found. Please " +
                            "reinstall Cryptnos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return true;
            }

            // Any key combinations that aren't recognized should pass up the chain to
            // whoever else might be listening:
            return false;
        }

        #region IUpdateCheckListener Methods

        public void OnFoundNewerVersion()
        {
            // This is pretty simple.  If the update check found a new version, tell it to
            // go ahead and download it.  Note that the update checker will handle any user
            // notifications, which includes a prompt on whether or not they'd like to
            // upgrade.  The null check is probably redudant--this method should never be
            // called if the update checker is null--but it's a belt-and-suspenders thing.
            try { if (updateChecker != null) updateChecker.GetNewerVersion(); }
            // If anything blows up, make sure to re-enable the button to let the user
            // try again:
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                ResetUpdateCheckButton(btnCheckForUpdatesText);
            }
        }

        public void OnNoUpdateFound()
        {
            // In the main app, if no update was found we silently go on about our business.  In
            // this instance, however, we want to explicitly notify the user.  Show the message,
            // then re-enable the button.
            MessageBox.Show("No new updates were found. You appear to have the latest version.",
                "Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetUpdateCheckButton(btnCheckForUpdatesText);
        }

        public void OnRecordLastUpdateCheck(DateTime lastCheck)
        {
            // For this one, we'll pass the buck up to the parent to let it store the last update
            // check into the registry:
            parent.OnRecordLastUpdateCheck(lastCheck);
        }

        public void OnRequestGracefulClose()
        {
            // There's not much we ought to do here.  Hide this form and tell the parent to close.
            // Note that as this stands now, if the user made any changes to the settings, those
            // settings will *NOT* be saved.
            Hide();
            parent.OnRequestGracefulClose();
        }

        public void OnUpdateCheckError()
        {
            // The update checker will do most of its own error handling here.  For our purposes,
            // we just need to re-enable the button so the user can check again.
            ResetUpdateCheckButton(btnCheckForUpdatesText);
        }

        public void OnDownloadCanceled()
        {
            // If the user cancels the download, just reset the button:
            ResetUpdateCheckButton(btnCheckForUpdatesText);
        }

        #endregion

    }
}