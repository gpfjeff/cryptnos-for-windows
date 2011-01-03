/* AboutDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 17, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      (None)
 * REQUIRED BY:   Form1
 * 
 * The About dialog for Cryptnos.  Nothing fancy here.  It just takes a version string and a
 * string containing the copyright information.
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
    public partial class AboutDialog : Form
    {
        /// <summary>
        /// The AboutDialog constructor
        /// </summary>
        /// <param name="version">The version string generated by the parent</param>
        /// <param name="copyright">The copyright string generated by the parent</param>
        /// <param name="showTooltips">A boolean value specifying whether or not to show
        /// tooltip help.</param>
        public AboutDialog(string versionLong, string versionShort, string copyright,
            bool showTooltips)
        {
            InitializeComponent();
            // Populate the title bar, version and copyright labels with information
            // provided to us by the parent.
            Text = "About " + versionShort;
            lblVersion.Text = versionLong;
            lblCopyright.Text = copyright;
            // Should we show tooltip help?
            toolTip1.Active = showTooltips;
        }

        /// <summary>
        ///  What to do when the OK button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}