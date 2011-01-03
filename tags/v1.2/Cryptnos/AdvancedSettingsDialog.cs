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
 * This program is Copyright 2010, Jeffrey T. Darlington.
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
        /// The currently selected text encoding
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="encoding">The current text encoding setting</param>
        public AdvancedSettingsDialog(Encoding encoding)
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
        }

        /// <summary>
        /// What to do when the OK button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Return an OK result and set the encoding to the selected value:
            DialogResult = DialogResult.OK;
            encoding = (Encoding)cmbTextEncodings.SelectedItem;
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
    }
}