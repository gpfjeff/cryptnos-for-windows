/* ImportDialog.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          October 12, 2011
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      (None)
 * REQUIRED BY:   Form1.cs
 * 
 * This form represents the Cryptnos import dialog, from which users can select individual site
 * parameters from an export file to import.  Prior to version 1.3.0, importing a file was an
 * all-or-nothing thing; if a site in the file already existed in the registry, the copy in the
 * registry was overwritten with the new version.  If the user only wanted to import a subset of
 * their parameter collection, they would have to be selective about which sites they exported
 * to begin with.
 * 
 * Starting with 1.3.0, the user is now presented with this dialog after the sites have been
 * decrypted.  This dialog lists the names for each site in the file with a checkbox next to
 * each one.  The user can select each site individually, or they may tick a "select all" box
 * that will toggle all of the sites on at once.  Sites from the file that will overwrite an
 * existing site will be shown in red so the user can quickly identify them.  (If the user
 * selects these sites for import, they will still overwrite the old values.)  Once the sites to
 * be imported are selected, the user clicks the Import button and the filtered list gets sent
 * back to the main window to be added to the registry.
 * 
 * UPDATES FOR 1.3.3: Pressing F1 now launches the HTML help.
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// The ImportDialog allows the user to pick and choose which sites they would like to import
    /// from a Cryptnos export file.
    /// </summary>
    public partial class ImportDialog : Form
    {
        /// <summary>
        /// Debug mode
        /// </summary>
        private bool debug = false;

        /// <summary>
        /// This boolean works around a quirk in the .NET window handling.  This should be set to
        /// true if the user clicks the Import button; otherwise, this should be false.
        /// </summary>
        private bool clickedImport = false;

        /// <summary>
        /// The <see cref="List"/> of <see cref="SiteParameters"/> imported from the file.  This will
        /// be our starting point from which we'll select which sites to import.
        /// </summary>
        private List<SiteParameters> sitesFromFile = null;

        /// <summary>
        /// A reference to the main Cryptnos window
        /// </summary>
        private MainForm caller = null;

        /// <summary>
        /// This read-only <see cref="List"/> of <see cref="SiteParameters"/> contains the user's
        /// selection of site parameters to import from the file.  Note that this property may be
        /// null if no sites were selected.
        /// </summary>
        public List<SiteParameters> SiteParams
        {
            get { return sitesFromFile; }
        }

        /// <summary>
        /// The main constructor
        /// </summary>
        /// <param name="caller">A reference to the main Cryptnos window</param>
        /// <param name="importedSites">A <see cref="List"/> of <see cref="SiteParameters"/> imported
        /// from a file. This is our starting list from which the user will select which sites to
        /// import</param>
        /// <param name="debug">Whether or not we are in debug mode</param>
        /// <param name="keepOnTop">Whether or not this window should remain on top of other windows</param>
        /// <param name="showToolTips">Whether or not to show tool tip help</param>
        public ImportDialog(MainForm caller, List<SiteParameters> importedSites, bool debug, bool keepOnTop,
            bool showToolTips)
        {
            // Initialize the window and grab local copies of all our inputs:
            InitializeComponent();
            this.caller = caller;
            sitesFromFile = importedSites;
            this.debug = debug;
            this.TopMost = keepOnTop;
            toolTip1.Active = showToolTips;
            // By default, disable the Import button until something has been selected:
            btnImport.Enabled = false;
            // If we got any sites from the import file, populate the site list here.  We will display the
            // list as checkboxes so the user can pick and choose which sites they want to import.  We also
            // want to indicate which sites will overwrite existing sites by coloring them red.
            if (importedSites != null && importedSites.Count > 0)
            {
                // First, get the list of existing sites from the caller:
                string[] existingSites = caller.GetSiteList();
                // Loop through the sites imported from the file:
                foreach (SiteParameters site in importedSites)
                {
                    // Create the checkbox for this site:
                    ListViewItem item = new ListViewItem(site.Site);
                    // If the site already exists, color the text for this item red.  Otherwise, we'll
                    // default to the regular color (most likely black).
                    if (SiteAlreadyExists(existingSites, site.Site))
                        item.ForeColor = Color.Red;
                    // Add the item to the list box:
                    listSitesInFile.Items.Add(item);
                }
            }
            // This should never happen, but if we didn't get any useful sites to work with, complain and
            // close the form:
            else
            {
                MessageBox.Show("No sites were found in the selected file", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Hide();
            }
        }

        /// <summary>
        /// What to do when the Cancel button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // This should be obvious, I hope:
            DialogResult = DialogResult.Cancel;
            sitesFromFile = null;
            Hide();
        }

        /// <summary>
        /// What to do when the dialog is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the user did not click the import button, default to cancel:
            if (!clickedImport)
            {
                DialogResult = DialogResult.Cancel;
                sitesFromFile = null;
            }

        }

        /// <summary>
        /// What to do when the Select All checkbox is toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            // If the checkbox has been checked...
            if (chkSelectAll.Checked)
            {
                // Loop through all the items in the list box and check them all:
                foreach (ListViewItem item in listSitesInFile.Items)
                    item.Checked = true;
                // Disable the list box to prevent anything from being unchecked:
                listSitesInFile.Enabled = false;
            }
            // Otherwise, if the checkbox has been cleared, enable the list box
            // so the items can be individually toggled.  Note that this does not
            // toggle any of the individual items themselves; if the user wants to
            // uncheck them all, they'll need to do each one, one at a time.
            else listSitesInFile.Enabled = true;
        }

        /// <summary>
        /// What to do if an individual item has been checked in the list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listSitesInFile_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // This should be ridiculously easy.  Look at the CheckedItems property of
            // the list box.  This is the list of items that have been checked.  If that
            // count is greater than zero, enable the Import button.  Otherwise,
            // disable it.
            btnImport.Enabled = listSitesInFile.CheckedItems.Count > 0;
        }

        /// <summary>
        /// What to do when the Import button has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            // Before we start importing, we want to warn the user if they're about to import a site
            // that will overwrite an exsiting one.  To do that, we'll need to look at the sites the
            // user has selected and see if any of them have red text.  If the text is red, it's a
            // site that will overwrite something.  Start by setting an "OK flag" to true, then loop
            // through the sites.  If we find a red one, set the flag to false and move on to the
            // next step.
            bool okToImport = true;
            foreach (ListViewItem item in listSitesInFile.CheckedItems)
            {
                if (item.ForeColor == Color.Red)
                {
                    okToImport = false;
                    break;
                }
            }
            // If the flag is false, we know we'll overwrite something.  Ask the user for confirmation
            // and set the flag to the value of their choice.
            if (!okToImport) okToImport = MessageBox.Show("You have selected at least one site to import " +
                "that will overwrite an existing site already saved in Cryptnos.  Overwriting an exsiting " +
                "site cannot be undone.  Are you sure you wish to import these sites?", "Overwrite Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
            // If we got this far, either we weren't going to overwrite anything to begin with or the
            // user said it was OK:
            if (okToImport)
            {
                // Create a new list of site parameters.  This will be the list that we will
                // actually send back to the main window to import into the registry.
                List<SiteParameters> newSiteList = new List<SiteParameters>();
                // Now loop through the checked items in the list box:
                foreach (ListViewItem item in listSitesInFile.CheckedItems)
                {
                    // Loop through the list of site parameters we got from the file:
                    foreach (SiteParameters site in sitesFromFile)
                    {
                        // Compare the site name to the list item text, which will be
                        // the name of the site.  If the two match, add the site to the
                        // list to be returned and remove it from the list from the file
                        // (to keep from having to search through it again later).  Then
                        // break out of the inner loop, as there's no point continuing
                        // the search.
                        if (site.Site.CompareTo(item.Text) == 0)
                        {
                            newSiteList.Add(site);
                            sitesFromFile.Remove(site);
                            break;
                        }
                    }
                }
                // Now look at the new list of sites.  The count should never be zero as the import
                // button should never get clicked if nothing was selected, but we'll do this as a
                // sanity check.  If we got anything, assign the new list of parameters to the
                // property we'll return to the caller, then close out the dialog, indicating success.
                if (newSiteList.Count > 0)
                {
                    sitesFromFile = newSiteList;
                    DialogResult = DialogResult.OK;
                    clickedImport = true;
                    Hide();
                }
            }
        }

        /// <summary>
        /// Search through a string array of existing site names and see if a specific site is
        /// in the list
        /// </summary>
        /// <param name="existingSites">The array of strings to search</param>
        /// <param name="newSite">The site string to find</param>
        /// <returns></returns>
        private bool SiteAlreadyExists(string[] existingSites, string newSite)
        {
            // If either of the inputs are null, go ahead and return false:
            if (existingSites == null || newSite == null) return false;
            // Loop through the array and search for the site.  Note that if we find it, we go
            // ahead and return true, ending the search early.
            foreach (string site in existingSites)
                if (newSite.CompareTo(site) == 0)
                    return true;
            // If we got here, the string wasn't found:
            return false;
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
