/* ExportSitesForm.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          November 6, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      (None)
 * REQUIRED BY:   Form1.cs
 * 
 * The Export Sites from for Cryptnos.  Initially, Cryptnos exported all sites whenever an
 * export was initiated.  After a good bit of thought, I realized this may not always be
 * desirable.  Exporting all sites may be useful when performing a complete backup or when
 * moving all your exported sites from one machine to another.  That said, I came up with
 * plenty of scenarios for exporting only a subset of sites:  copying only the sites that had
 * recently changed, selecting only the sites you want someone else to have access to, etc.
 * A new interface was required to give the user the ability to pick and choose.
 * 
 * This dialog does just that.  There are two main options:  Export all sites, or export some.
 * If export some is chosen, a list box contains the list of site tokens from the main form
 * to allow the user to select which sites they want to export.  This is a multi-select box,
 * so you can select any combination of site tokens you wish.  Once the user decides which
 * sites they want to export, they can click the Export button to proceed to the next step
 * (choosing an export passphrase).  If the user clicks Cancel, the process is aborted.
 * 
 * UPDATES FOR 1.3.0:  Added new tabbed layout.  Original export form is now on the tab
 * "To File" and is relatively unchanged.  The new second tab is "To QR Code", which allows
 * us to export parameters to a QR Code which can then be read by the Cryptnos Android client.
 * 
 * UPDATES FOR 1.3.3: Pressing F1 now launches the HTML help.
 * 
 * UPDATES FOR 1.3.4: Tweaks to improve behavior under Mono.
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
using System.Windows.Forms;
using com.google.zxing;
using com.google.zxing.common;
using com.google.zxing.qrcode;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// The ExportSitesForm provides a GUI to allow the user to chose which site parameters
    /// they want to export.  The user can choose to export all their sites or just a subset
    /// of them.  If they do not cancel the process, the selected sites will be available
    /// through the SelectedSites object array.
    /// </summary>
    public partial class ExportSitesForm : Form
    {
        /// <summary>
        /// A convenience object array that will eventually hold the sites to export.  This
        /// is the internal representation of the SelectedSites property.
        /// </summary>
        private object[] siteArray = null;

        /// <summary>
        /// Whether or not the OK button was clicked.  This is used to determine the behavior
        /// of the dialog as it is being closed.
        /// </summary>
        private bool clickedOK = false;

        /// <summary>
        /// A reference to the <see cref="MainForm"/> object that called us
        /// </summary>
        private MainForm caller = null;

        /// <summary>
        /// A ZXing <see cref="QRCodeWriter"/> for generating QR Codes
        /// </summary>
        private QRCodeWriter qrCodeWriter = new QRCodeWriter();

        /// <summary>
        /// A ZXing <see cref="ByteMatrix"/> for converting QR Code data to an image
        /// </summary>
        private ByteMatrix byteMatrix = null;

        /// <summary>
        /// A read-only property consisting of an array of objects (really strings) comprising
        /// the list of sites selected for export.  Note that if the user cancels the export
        /// process, this array may be null.
        /// </summary>
        public object[] SelectedSites
        {
            get { return siteArray; }
        }

        /// <summary>
        /// The ExportSitesForm constructor.
        /// </summary>
        /// <param name="sites">An object array containing the strings of the site tokens,
        /// which should be taken from the Sites combo box on the main Cryptnos form.</param>
        /// <param name="showTooltips">A boolean value specifying whether or not to show
        /// tooltip help.</param>
        /// <param name="keepOnTop">Keep Cryptnos on top of other windows</param>
        public ExportSitesForm(object[] sites, bool showTooltips, bool keepOnTop, MainForm caller)
        {
            // The normal initializaition:
            InitializeComponent();
            // Populate the site list box.  The sites array should contain a bunch of strings
            // corresponding to the items in the Sites combo box on the main form.  For our
            // purposes, it technically doesn't matter if they're strings or not, but that's
            // what we'll be getting and passing back out.  However, since both the combo box
            // and list box Items properties return ObjectCollection objects, it's easier to
            // just use the ObjectCollection.CopyTo() method to get an object array to deal
            // with.
            foreach (Object site in sites)
            {
                lbSiteList.Items.Add(site);
            }
            // By default, make the Export All option be enabled:
            rbExportAll.Checked = true;
            rbExportSome.Checked = false;
            lbSiteList.Enabled = false;
            // Populate the site drop-down on the QR code tab:
            cmboExportSiteQR.Items.AddRange(sites);
            // Turn tooltips on or off depending on the value passed in from the main form:
            toolTip1.Active = showTooltips;
            TopMost = keepOnTop;
            this.caller = caller;
        }

        /// <summary>
        /// What to do when the Export All radio button is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbExportAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbExportAll.Checked) SwitchToExportAll();
            else SwitchToExportSome();
        }

        /// <summary>
        /// What to do when the Export Some radio button is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbExportSome_CheckedChanged(object sender, EventArgs e)
        {
            if (rbExportSome.Checked) SwitchToExportSome();
            else SwitchToExportAll();
        }
        
        /// <summary>
        /// What to do when the Export button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            // Export All is the easiest to deal with:
            if (rbExportAll.Checked)
            {
                // Populate the siteArray (and thus SelectedSites) with the entire list
                // in the lsit box:
                siteArray = new object[lbSiteList.Items.Count];
                lbSiteList.Items.CopyTo(siteArray, 0);
                // Return to the main form and tell them to proceed:
                clickedOK = true;
                DialogResult = DialogResult.OK;
                Hide();
            }
            // Export Some is a tiny bit more complicated:
            else
            {
                // As long as there are some sites selected, this is almost identical to
                // the Export All code above.  The big difference is that we'll use the
                // SelectedItems property rather than the Items property on the list box
                // to just get the items the user selected:
                if (lbSiteList.SelectedItems.Count > 0)
                {
                    siteArray = new object[lbSiteList.SelectedItems.Count];
                    lbSiteList.SelectedItems.CopyTo(siteArray, 0);
                    clickedOK = true;
                    DialogResult = DialogResult.OK;
                    Hide();
                }
                // Otherwise, if no sites were selected, we'll assume the user made a
                // boo-boo.  Tell them what they need to do:
                else MessageBox.Show("You have chosen to export only some of your site " +
                  "paramemters, but you haven't chosen any sites to export.  Please " +
                  "select at least once site.", "Warning", MessageBoxButtons.OK,
                  MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// What to do when the Cancel button is clicked:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // This should be obvious, I hope:
            DialogResult = DialogResult.Cancel;
            siteArray = null;
            Hide();
        }

        /// <summary>
        /// What to do when the dialog is being closed:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSitesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // This is a little weird, I guess, but when I found out that calling Dispose()
            // wasn't going to send the result back to the caller, I opted to use Hide()
            // instead.  That meant that when we got to closing the form by other means
            // (i.e. the X in the upper right corner), things got a bit wonky.  If the user
            // clicked the Export button (see btnExport_Click()), the clickedOK boolean flag
            // should be set to true and the result should already be set to OK.  Otherwise, 
            // clickedOK defaults to false but the result is unset.  So explicitly set the
            // result to Cancel here and make sure the passphrase property is cleared out.
            if (!caller.IsMono)
            {
                if (!clickedOK)
                {
                    DialogResult = DialogResult.Cancel;
                    siteArray = null;
                }
            }
        }

        /// <summary>
        /// Switch the dialog to Export All mode
        /// </summary>
        private void SwitchToExportAll()
        {
            // This should be built into the radio buttons, but if it is, I'm not finding
            // it.  Make sure that if the Export All radio button is checked, that the
            // Export Some radio button *isn't* checked and the list box is disabled.  Also
            // make sure the Export button is enabled.
            rbExportAll.Checked = true;
            rbExportSome.Checked = false;
            lbSiteList.Enabled = false;
            btnExport.Enabled = true;
        }

        /// <summary>
        /// Switch the dialog to Export Some mode
        /// </summary>
        private void SwitchToExportSome()
        {
            // The opposite of SwitchToExportAll():  If Export Some is checked, uncheck
            // Export All and enable the list box.  Enable/disable the Export button
            // depending on whether or not any sites are selected.
            rbExportAll.Checked = false;
            rbExportSome.Checked = true;
            lbSiteList.Enabled = true;
            if (lbSiteList.SelectedItems.Count > 0) btnExport.Enabled = true;
            else btnExport.Enabled = false;
        }

        /// <summary>
        /// What to do when selections change in the sites list box:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSiteList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Disable the Export button when no sites are selected, or enable it
            // when at least one is selected:
            if (lbSiteList.SelectedItems.Count > 0) btnExport.Enabled = true;
            else btnExport.Enabled = false;
        }

        /// <summary>
        /// What to do when the selection changes in the QR Code site list:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmboExportSiteQR_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Asbestos underpants:
            try
            {
                // Get the current selection of the combo box and make sure it
                // isn't empty:
                String site = (string)cmboExportSiteQR.Text;
                if (!String.IsNullOrEmpty(site))
                {
                    // Ask the main form to give us the site parameters for this site.
                    // Since the main form has all the code to do this, we'll ask it
                    // to do the dirty work.
                    SiteParameters siteParams = caller.GetSiteParamsForQRCode(site);
                    // Now we'll generate the text we'll embed into the QR Code.  We want
                    // this to be as compact as we can get it, so our "headings" will be
                    // single letters.  We'll start off with an identifying header so the
                    // QR Code reader will know the format of our string.  We delimite the
                    // string with pipes, which aren't allowed in any of our fields.  We
                    // want this to match as closely to the values of the XML export file
                    // for consistency.  That means the hash engine and the character
                    // limit fields will need some tweaking.
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CRYPTNOSv1|" +
                        "S:" + siteParams.Site + "|" +
                        "H:" + HashEngine.HashEnumStringToDisplayHash(siteParams.Hash) + "|" +
                        "I:" + siteParams.Iterations.ToString() + "|" +
                        "C:" + siteParams.CharTypes.ToString() + "|L:");
                    if (siteParams.CharLimit < 0) sb.Append("0");
                    else sb.Append(siteParams.CharLimit.ToString());
                    // Now that we've built our string, use the QRCodeWriter from ZXing to
                    // build the QR Code image and assign the bitmap to the picture box:
                    byteMatrix = qrCodeWriter.encode(sb.ToString(),
                        BarcodeFormat.QR_CODE, 200, 200);
                    pictureBox1.Image = byteMatrix.ToBitmap();
                }
                // If the selection in the combo box wasn't useful, empty the picture box:
                else pictureBox1.Image = null;
            }
            // Similarly, if anything blew up, empty the picture box:
            catch { pictureBox1.Image = null; }
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

    }
}