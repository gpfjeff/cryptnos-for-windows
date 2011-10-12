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
 * This program is Copyright 2011, Jeffrey T. Darlington.
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// This dialog allows the user to tweak advanced settings within Cryptnos, such as
    /// which text encoding to use during password generation.
    /// </summary>
    public partial class AdvancedSettingsDialog : Form
    {
        /// <summary>
        /// The currently selected text encoding
        /// </summary>
        private Encoding encoding = Encoding.Default;

        /// <summary>
        /// Whether or not debug mode is enabled
        /// </summary>
        private bool debug = false;

        /// <summary>
        /// Whether or not to disable the update check
        /// </summary>
        private bool disableUpdateCheck = false;

        /// <summary>
        /// Whether or not to force an update check on the next launch
        /// </summary>
        private bool forceUpdateCheck = false;

        /// <summary>
        /// Whether or not Cryptnos should stay on top of other windows
        /// </summary>
        private bool keepOnTop = false;

        /// <summary>
        /// Flag to determine whether or not we should show the disable update check warning.
        /// This should be initially true and should be shown at least once when the disable
        /// update check box is checked.
        /// </summary>
        private bool showDisableUpdateCheckWarning = true;

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
        /// Whether or not to force an update check on the next launch
        /// </summary>
        public bool ForceUpdateCheck
        {
            get { return forceUpdateCheck; }
        }

        /// <summary>
        /// Whether or not Cryptnos should stay on top of other windows
        /// </summary>
        public bool KeepOnTop
        {
            get { return keepOnTop; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="encoding">The current text encoding setting</param>
        /// <param name="showTooltips">A boolean value specifying whether or not to show
        /// tooltip help.</param>
        /// <param name="debug">Whether or not debug mode is enabled</param>
        /// <param name="disableUpdateCheck">Whether or not to disable the update check</param>
        /// <param name="keepOnTop">Whether or not to keep Cryptnos on top of other windows</param>
        public AdvancedSettingsDialog(Encoding encoding, bool showTooltips, bool debug,
            bool disableUpdateCheck, bool keepOnTop)
        {
            InitializeComponent();
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
            chkDebug.Checked = debug;
            chkDisableUpdateCheck.Checked = disableUpdateCheck;
            // The "keep on top" setting:
            this.keepOnTop = keepOnTop;
            chkKeepOnTop.Checked = keepOnTop;
            this.TopMost = keepOnTop;
            // If the update check is currently disabled, disable the force update check box
            // so it cannot be selected:
            if (disableUpdateCheck)
            {
                chkForceUpdateCheck.Checked = false;
                chkForceUpdateCheck.Enabled = false;
            }
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
            forceUpdateCheck = chkForceUpdateCheck.Checked;
            keepOnTop = chkKeepOnTop.Checked;
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
                // First, uncheck and disable the force update check box.  This isn't a valid
                // option if the update check is completely disabled.
                chkForceUpdateCheck.Checked = false;
                chkForceUpdateCheck.Enabled = false;
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
            // If the box is unchecked, all we have to do for now is re-enable the force
            // update check box, which is now a valid option:
            else chkForceUpdateCheck.Enabled = true;
        }
    }
}