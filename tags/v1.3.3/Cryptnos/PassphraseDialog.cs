/* PassphraseDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 17, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      (None)
 * REQUIRED BY:   Form1
 * 
 * The Cryptnos passphrase dialog box.  This provides a GUI for prompting and accepting the
 * user's passphrase, which will be passed back to the caller for further analysis.  This
 * dialog can be created in one of three "modes" which determines the prompt to be displayed.
 * It is intended to be called twice during the export process (once to get the initial pass-
 * phrase and a second time to confirm it) and once during import.  It returns the standard
 * Form.DialogResult property and exposes a read-only Passphrase property to allow access to
 * the entered passphrase.
 * 
 * This program is Copyright 2009, Jeffrey T. Darlington.
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
using System.Text;
using System.Windows.Forms;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// The PassphraseDialog provides a GUI for retrieving a passphrase from the user.  The
    /// passphrase is masked from view, protecting it from over-the-shoulder reading.
    /// </summary>
    public partial class PassphraseDialog : Form
    {
        /// <summary>
        /// The Mode enumeration indicates how the PassphraseDialog is intended to be used.
        /// It can be called in one of three ways:  (1) During export of parameters, prompt
        /// the user for the initial passphrase to be used for encryption; (2) also during
        /// export, prompt the user for the passphrase again to confirm that the two phrases
        /// match; (3) during import, prompt for the passphrase for decryption.  Which mode
        /// is used primarily determines the text displayed to the user.
        /// </summary>
        public enum Mode
        {
            Export_Initial,
            Export_Confirm,
            Import
        }

        /// <summary>
        /// The user's passphrase
        /// </summary>
        private string passphrase = null;

        /// <summary>
        /// Whether or not the OK button was clicked.  This is used to determine the behavior
        /// of the dialog as it is being closed.
        /// </summary>
        private bool clickedOK = false;

        /// <summary>
        /// The user's passphrase.  Note that this property is read-only.
        /// </summary>
        public string Passphrase
        {
            get { return passphrase; }
        }

        /// <summary>
        /// Primary constructor.  Given the appropriate mode, display the passphrase dialog.
        /// </summary>
        /// <param name="mode">The mode the dialog should be called in.  The mode determines
        /// what prompt text is displayed to the user.</param>
        /// <param name="keepOnTop">Keep Cryptnos on top of other windows</param>
        public PassphraseDialog(Mode mode, bool keepOnTop)
        {
            // Do the usual initialization:
            InitializeComponent();
            // Determine which prompt text to display:
            switch (mode)
            {
                case Mode.Export_Initial:
                    lblPrompt.Text = "Please enter a passphrase to protect your " +
                        "parameters:";
                    break;
                case Mode.Export_Confirm:
                    lblPrompt.Text = "Please re-enter your passphrase to confirm:";
                    break;
                default:
                    lblPrompt.Text = "Please enter your passphrase to unlock your parameters:";
                    break;
            }
            TopMost = keepOnTop;
        }

        /// <summary>
        /// DEPRECIATED constructor.  Use PassphraseDialog(Mode) instead.  This method was
        /// added before we added the Mode enumeration and the extra prompt on export.  It
        /// will be eventually removed.
        /// </summary>
        /// <param name="export">A boolean flag indicating the mode the dialog should be
        /// displayed in.  If true, put it in initial Export mode; if false, import
        /// mode.</param>
        /// <param name="keepOnTop">Keep Cryptnos on top of other windows</param>
        public PassphraseDialog(bool export, bool keepOnTop)
            : this(export ? Mode.Export_Initial : Mode.Import, keepOnTop)
        {
        }

        /// <summary>
        /// What to do when the OK button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Don't let them click OK if there's nothing the passphrase box:
            if (String.IsNullOrEmpty(txtPassphrase.Text))
            {
                MessageBox.Show("Please enter a passphrase", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Otherwise, take note of the passphrase and return an OK result to the caller:
            else
            {
                passphrase = txtPassphrase.Text;
                DialogResult = DialogResult.OK;
                Hide();
                clickedOK = true;
            }
        }

        /// <summary>
        /// What to do when the Cancel button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // This *should* be obvious:
            DialogResult = DialogResult.Cancel;
            passphrase = null;
            Hide();
        }

        /// <summary>
        /// What to do when the dialog is being closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PassphraseDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // This is a little weird, I guess, but when I found out that calling Dispose()
            // wasn't going to send the result back to the caller, I opted to use Hide()
            // instead.  That meant that when we got to closing the form by other means
            // (i.e. the X in the upper right corner), things got a bit wonky.  If the user
            // clicked the OK button (see btnOK_Click()), the clickedOK boolean flag should
            // be set to true and the result should already be set to OK.  Otherwise, 
            // clickedOK defaults to false but the result is unset.  So explicitly set the
            // result to Cancel here and make sure the passphrase property is cleared out.
            if (!clickedOK)
            {
                DialogResult = DialogResult.Cancel;
                passphrase = null;
            }
        }


    }
}