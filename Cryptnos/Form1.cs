/* Form1.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 17, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      AboutDialog, PassphraseDialog, ExportSitesForm
 * REQUIRED BY:   (None)
 * 
 * The main Cryptnos application form.  Cryptnos is a small .NET GUI utility for generating
 * strong, unique yet repeatable passphrases using cryptographic hashes.  Its original intended
 * use is for website authentication, but it can be used for any purpose where strong
 * passphrases are required or encouraged.  Cryptnos combines an easy-to-remember token with
 * a secret phrase known only to the user, then passes both to a selectable cryptographic hash
 * to generate a passphrase that is seemingly random, difficult to brute-force, and impossible
 * to regenerate without knowing both the token and the original secret.
 * 
 * Another advantage of Cryptnos is that it allows you to tweak your generated passphrase to
 * fit the constraints of the target site.  For example, some websites require passphrases to
 * be within certain length constraints (say 8-12 characters) or may limit the types of
 * characters that can be used (for example, only alphanumerics).  Cryptnos will generate the
 * hash from the original material and then apply these limits.  For example, you can use a
 * very strong SHA-512 hash, limit it to only alphanumerics, and then limit it further to only
 * 12 characters long.  The final generated hash is suitable for copying or typing into the
 * website's form without further modification.
 * 
 * Best of all, Cryptnos remembers the the complex rules for generating each passphrase and
 * stores it in a secure fashion in the Windows registry.  It uses both cryptographic hashes
 * and strong AES (Rijndael) encryption to store the rules for each passphrase in the registry,
 * so even if the registry is copied or read by an admin, the values cannot be easily read.
 * Of course, the final generated password requires the user's secret, which is *NEVER* stored,
 * so even if the site parameters in the registry are decrypted, the final passphrase cannot
 * be generated without social engineering or similar external means.
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Win32;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// The main Cryptnos application form
    /// </summary>
    public partial class Form1 : Form
    {
        #region Private Variables

        /// <summary>
        /// The full version number of the assmebly for display
        /// </summary>
        private static string version = "Cryptnos v. " +
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// The shortened (major + minor) version number for display
        /// </summary>
        private static string versionShort = "Cryptnos v. " +
            Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." +
            Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();

        /// <summary>
        /// The copyright string for the About dialog
        /// </summary>
        string copyright = "";

        /// <summary>
        /// A <see cref="RegistryKey"/> object for getting quick access to the Cryptnos
        /// registry key.
        /// </summary>
        private RegistryKey CryptnosSettings = null;

        /// <summary>
        /// A <see cref="RegistryKey"/> that is a subkey of <see cref="CryptnosSettings"/>
        /// that contains the actual site parameter values.
        /// </summary>
        private RegistryKey siteParamsKey = null;

        /// <summary>
        /// A <see cref="Hashtable"/> holding the length of the output string for each
        /// <see cref="HashAlgorithm"/> available through WinHasher's <see cref="HashEngine"/>.
        /// </summary>
        private Hashtable hashLengths = null;

        /// <summary>
        /// A string containing the last path used for import/export operations.  This should
        /// default to My Documents when the app first opens, but should be updated each time
        /// an import/export operation is performed.
        /// </summary>
        private string lastImportExportPath = null;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            // Begin with the default initialization:
            InitializeComponent();
            // Default to using all the available characters in the hash:
            cbCharTypes.SelectedIndex = 0;
            // Bind the hash drop-down to the WinHasher Hashes enum and force SHA-1 to
            // be the default (it can be overridden below):
            cbHashes.DataSource = Enum.GetNames(typeof(Hashes));
            cbHashes.SelectedItem = Hashes.SHA1.ToString();
            // Now put on our asbestos underpants, because now we're playing with
            // dynamite:
            try
            {
                // Attempt to open the Cryptnos registry key:
                if (CryptnosRegistryKeyOpen())
                {
                    // If that worked, get the state of the remember parameters value:
                    chkRemember.Checked = (int)CryptnosSettings.GetValue("RememberParams", 1)
                        == 1 ? true : false;
                    // If we're remembering things, try to get any settings they've saved
                    // from the registry so they're ready to go:
                    if (chkRemember.Checked)
                    {
                        PopulateSitesDropdown();
                        GetLastSiteParams();
                        if (cbSites.Items.Count > 0)
                        {
                            btnForget.Enabled = true;
                            btnForgetAll.Enabled = true;
                            btnExport.Enabled = true;
                        }
                        else
                        {
                            btnForget.Enabled = false;
                            btnForgetAll.Enabled = false;
                            btnExport.Enabled = false;
                        }
                        chkLock.Enabled = true;
                    }
                    // Otherwise, set some sensible defaults and clear out the sites drop-down:
                    else
                    {
                        cbSites.Items.Clear();
                        btnForget.Enabled = false;
                        btnForgetAll.Enabled = false;
                        btnExport.Enabled = false;
                        chkLock.Enabled = false;
                    }
                    // If that worked, get the state of the lock parameters value:
                    chkLock.Checked = (int)CryptnosSettings.GetValue("LockParams", 0)
                        == 1 ? true : false;
                    // Get the user's tooltip help preference:
                    chkShowTooltips.Checked = (int)CryptnosSettings.GetValue("ToolTips", 1)
                        == 1 ? true : false;
                }
            }
            // If anything above blew up, set some sensible defaults:
            catch
            {
                chkRemember.Checked = true;
                chkLock.Checked = false;
                cbCharTypes.SelectedIndex = 0;
                cbSites.Text = String.Empty;
                txtPassphrase.Text = String.Empty;
                txtCharLimit.Text = String.Empty;
                txtIterations.Text = "1";
                cbHashes.SelectedItem = Hashes.SHA1.ToString();
                btnForget.Enabled = false;
                btnForgetAll.Enabled = false;
                btnExport.Enabled = false;
                chkShowTooltips.Checked = true;
            }
            // Turn on or off tooltip help depending on the user's save preference:
            toolTip1.Active = chkShowTooltips.Checked;
            // Generate the hash lengths list.  The idea here is to enforce the character
            // limit ranges depending on which hash algorithm has been selected.  To do that,
            // we'll quickly generate each available hash and get the length of the output
            // string, then save all these to a Hashtable so we can quickly look it up later.
            hashLengths = new Hashtable();
            foreach (Hashes item in Enum.GetValues(typeof(Hashes)))
            {
                string tmp = HashEngine.HashString(item, "null", 1);
                hashLengths.Add(item, tmp.Length);
            }
            // Set the window title to include the short version number:
            Text = versionShort;
            // Get our copyright information.  It seems a bit silly to do it this way,
            // but this seems to be the only way to do it that I can find.  We'll pull this
            // from the assembly so we only need to change it in one place, and it can be
            // automatically fetched from SVN.
            object[] obj = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (obj != null && obj.Length > 0)
                copyright = ((AssemblyCopyrightAttribute)obj[0]).Copyright;

            // Default the last import/export path to My Documents (or its equivalent) or,
            // if that fails, the current working directory:
            try { lastImportExportPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
            catch { lastImportExportPath = Environment.CurrentDirectory; }
        }

        #region GUI Event Handlers

        /// <summary>
        /// What to do if the sites drop-down loses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSites_Leave(object sender, EventArgs e)
        {
            // If we've elected to save our settings and there's currently a site in the
            // editable field, go ahead and load the current settings from the registry:
            if (chkRemember.Checked && !String.IsNullOrEmpty((string)cbSites.Text))
                GetSiteParamsBySite(cbSites.Text);
        }

        /// <summary>
        /// What to do when the site drop-down selection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we've elected to save our settings, go ahead and load the current settings
            // for the selected site from the registry:
            if (chkRemember.Checked && !String.IsNullOrEmpty((string)cbSites.Text))
                GetSiteParamsBySite(cbSites.Text);
        }

        /// <summary>
        /// What to do when the Generate button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // Asbestos underpants:
            try
            {
                // Go ahead and get the selected hash algorithm now.  Then find out the
                // length of the generated hash and hold on to that value for now.
                Hashes hashAlgo = (Hashes)Enum.Parse(typeof(Hashes),
                        (string)cbHashes.SelectedItem);
                int hashLength = (int)hashLengths[hashAlgo];
                // Error checking:  Since the check is more complex, start by checking the
                // iteration count text box.  Try parsing the text value of that box and
                // stuffing it into an integer value.  If the parse fails, or if the integer
                // is equal to or less than zero, throw an error:
                int iterations = 1;
                bool parsedIterations = Int32.TryParse(txtIterations.Text, out iterations);
                if (!parsedIterations || iterations < 1)
                {
                    MessageBox.Show("The iteration count must be a positive integer greater " +
                        "than zero.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    txtIterations.Focus();
                }
                // Make sure the site field isn't empty:
                else if (String.IsNullOrEmpty((string)cbSites.Text))
                {
                    MessageBox.Show("The site box is empty.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    cbSites.Focus();
                }
                // Obviously, the passphrase box shouldn't be empty either:
                else if (String.IsNullOrEmpty(txtPassphrase.Text))
                {
                    MessageBox.Show("The passphrase box is empty.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    txtPassphrase.Focus();
                }
                // If the character limit box is populated, make sure it's only digits
                // (i.e. positive integers):
                else if (!String.IsNullOrEmpty(txtCharLimit.Text) &&
                    !Regex.IsMatch(txtCharLimit.Text, @"^\d+$"))
                {
                    MessageBox.Show("The character limit box must only contain numbers.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCharLimit.Focus();
                }
                // Likewise, the character limit must be between one (since zero characters
                // is completely useless) and the max size of the generated hash:
                else if (!String.IsNullOrEmpty(txtCharLimit.Text) && 
                    (Int32.Parse(txtCharLimit.Text) > hashLength ||
                    Int32.Parse(txtCharLimit.Text) <= 0))
                {
                    MessageBox.Show("The character limit, if set, must be between 1 and " +
                        hashLength.ToString() + " characters.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCharLimit.Focus();
                }
                // If all those are OK, move on to the actual work:
                else
                {
                    // Generate the hash.  We'll take advantage of WinHasher's HashEngine
                    // here to do the grunt work.  For the text, combine the site name with
                    // the passphrase text to give us a unique plaintext input.  Note that
                    // the output is Base64 to give us a healthy output.
                    //string hash = HashEngine.HashText(hashAlgo, cbSites.Text +
                    //    txtPassphrase.Text, Encoding.Default, OutputType.Base64);
                    string hash = HashEngine.HashString(hashAlgo, Encoding.Default,
                        cbSites.Text + txtPassphrase.Text, iterations);
                    // Now that we have the hash, we'll wittle it down based on the user's
                    // preferences.  First we'll eliminate any undesireable character types:
                    switch ((string)cbCharTypes.SelectedItem)
                    {
                        case "alphanumerics, change others to underscores":
                            hash = Regex.Replace(hash, @"\W", "_");
                            break;
                        case "alphanumerics only":
                            hash = Regex.Replace(hash, @"[^a-zA-Z0-9]", "");
                            break;
                        case "alphabetic characters only":
                            hash = Regex.Replace(hash, @"[^a-zA-Z]", "");
                            break;
                        case "numbers only":
                            hash = Regex.Replace(hash, @"\D", "");
                            break;
                        default:
                            // By default, use all generated characters
                            break;
                    }
                    // Now that we've eliminated unwanted characters, we'll work on the
                    // length.  If the character limit has been set and it is less than
                    // the current length of the hash, crop the hash to the desired length:
                    if (!String.IsNullOrEmpty(txtCharLimit.Text) &&
                        Int32.Parse(txtCharLimit.Text) < hash.Length)
                        hash = hash.Substring(0, Int32.Parse(txtCharLimit.Text));
                    // Now that the hash has been generated and tweaked, display it in the
                    // password box:
                    txtPassword.Text = hash;
                    // If the user has elected to save their settings, do so now:
                    if (chkRemember.Checked && !chkLock.Checked) SaveSiteParams(cbSites.Text);
                }
            }
            // If any errors occur, show an error message.  We should really make this more
            // robust and user-friendly.
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do when the Close button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // This method contains the common clean-up code, so call it:
            ExitApplication();
            Dispose();
        }

        /// <summary>
        /// What to do if the user clicks the form's close button (the X)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // This method contains the common clean-up code, so call it:
            ExitApplication();
        }

        /// <summary>
        /// What to do when the Remember Parameters checkbox state changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkRemember_CheckedChanged(object sender, EventArgs e)
        {
            // What we do here depends on the new state of the checkbox:
            if (chkRemember.Checked)
            {
                // If we're now remembering things, take note of the current site and
                // repopulate the site drop-down.  Note this will clear out the current
                // site text area, which is why we're pulling that out on its own.
                string currentSite = cbSites.Text;
                PopulateSitesDropdown();
                // If the site is in the registry, select it in the drop down and grab
                // its data:
                if (cbSites.Items.IndexOf(currentSite) != -1)
                {
                    cbSites.SelectedIndex = cbSites.Items.IndexOf(currentSite);
                    GetSiteParamsBySite(currentSite);
                }
                // Otherwise, just put other site back in the box:
                else cbSites.Text = currentSite;
                // Re-enable the Lock checkbox:
                chkLock.Enabled = true;
            }
            else
            {
                // For now, if we're not remembering anything anymore, just clear out the
                // drop-down list but don't change anything else.  We'll add more here if
                // we deem it relevant.
                cbSites.Items.Clear();
                // Uncheck and disable the Lock checkbox.  If they're not remembering
                // anything, they shouldn't be able to lock it.
                chkLock.Checked = false;
                chkLock.Enabled = false;
            }
        }

        /// <summary>
        /// What to do when the About button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            // Create and open the about dialog:
            AboutDialog ad = new AboutDialog(version, versionShort, copyright,
                chkShowTooltips.Checked);
            ad.ShowDialog();
        }

        /// <summary>
        /// What to do when the Import button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            // Asbestos underpants:
            try
            {
                // Don't let them change their settings if they've locked the parameters:
                if (chkRemember.Checked && chkLock.Checked)
                {
                    MessageBox.Show("You currently have your site parameters locked. You " +
                        "must unlock your site parameters for this change to take effect.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnForget.Enabled = false;
                    btnForgetAll.Enabled = false;
                }
                else
                {
                    // If the user imports settings from a file and that file contains a site that
                    // already exists in the registry, the imported site will overwrite the existing
                    // site.  Warn the user that this will happen and get their go-ahead before we
                    // proceed.
                    if (MessageBox.Show("Please note:  If the import file contains a site that already " +
                        "exists in your site parameters, the site parameters in the registry will " +
                        "be overwritten by the ones from the file.  Are you sure you wish to " +
                        "proceed?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                        DialogResult.Yes)
                    {
                        // This only makes sense if we can open the registry:
                        if (CryptnosRegistryKeyOpen())
                        {
                            // Prompt the user for an import file:
                            OpenFileDialog ofd = new OpenFileDialog();
                            ofd.InitialDirectory = lastImportExportPath;
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                // Take note of the file name, as well as update the last
                                // import/export path for later use:
                                string filename = ofd.FileName;
                                try { lastImportExportPath =
                                    (new FileInfo(filename)).DirectoryName; }
                                catch { }
                                // Prompt the user for their passphrase (Cryptnos files are always
                                // encrypted):
                                PassphraseDialog pd =
                                    new PassphraseDialog(PassphraseDialog.Mode.Import);
                                if (pd.ShowDialog() == DialogResult.OK)
                                {
                                    string passphrase = pd.Passphrase;
                                    pd.Dispose();
                                    // Read all the raw data of the import file.  Note that
                                    // SecureFile files are always encrypted.
                                    byte[] rawData = SecureFile.ReadAllBytes(filename, passphrase);
                                    // The raw data should contain a binary formatted generic List
                                    // of SiteParameter objects.  So attempt to deserialize that
                                    // List:
                                    BinaryFormatter bf = new BinaryFormatter();
                                    MemoryStream ms = new MemoryStream(rawData);
                                    List<SiteParameters> siteParamList =
                                        (List<SiteParameters>)bf.Deserialize(ms);
                                    ms.Close();
                                    // If the deserialization didn't blow up, step through the list
                                    // and save each set of parameters to the registry.  Note that
                                    // if a given site already exists in the registry, this will
                                    // overwrite it.
                                    foreach (SiteParameters siteParam in siteParamList)
                                        siteParam.SaveToRegistry(siteParamsKey);
                                    // Now regenerate the sites drop-down list with the newly
                                    // imported data and let the user know we were successful.
                                    PopulateSitesDropdown();
                                    MessageBox.Show("The parameters have been successfully " +
                                        "imported from the file.", "Information",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    btnExport.Enabled = true;
                                    btnForgetAll.Enabled = true;
                                    btnForget.Enabled = true;
                                }
                            }
                        }
                        // The registry keys weren't open:
                        else MessageBox.Show("I could not open the registry to get your site parameter " +
                            "options, so I can't import anything!", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            // If something blew up, notify the user.  This needs to be more robust with more
            // user-friendly messages.
            catch
            {
                MessageBox.Show("I was unable to import site parameters from the specified " +
                    "file.  Make sure you have entered your passphrase correctly and that " +
                    "the file has not been corrupted.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do when the Export button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            // Asbestos underpants:
            try
            {
                // This only makes sense if the registry keys are open...
                if (CryptnosRegistryKeyOpen())
                {
                    // ... and if there are any actual sites saved in the registry:
                    if (siteParamsKey.GetValueNames().Length > 0)
                    {
                        // Open up the export dialog and let the user select what sites
                        // they want to export:
                        object[] sitesToExport = new object[cbSites.Items.Count];
                        cbSites.Items.CopyTo(sitesToExport, 0);
                        ExportSitesForm esf = new ExportSitesForm(sitesToExport,
                            chkShowTooltips.Checked);
                        // If they selected anything to export:
                        if (esf.ShowDialog() == DialogResult.OK && esf.SelectedSites != null
                            && esf.SelectedSites.Length > 0)
                        {
                            sitesToExport = esf.SelectedSites;
                            // Prompt the user for a passphrase.  All Cryptnos export files are
                            // encrypted, so we need a passphrase to encrypt the data with.
                            PassphraseDialog pd =
                                new PassphraseDialog(PassphraseDialog.Mode.Export_Initial);
                            if (pd.ShowDialog() == DialogResult.OK)
                            {
                                string passphrase = pd.Passphrase;
                                pd.Dispose();
                                // Now prompt them for their passphrase again to make sure they
                                // don't misstype it:
                                pd = new PassphraseDialog(PassphraseDialog.Mode.Export_Confirm);
                                if (pd.ShowDialog() == DialogResult.OK)
                                {
                                    // If the passphrases match:
                                    if (passphrase == pd.Passphrase)
                                    {
                                        // Now prompt the user for a file to save to:
                                        SaveFileDialog sfd = new SaveFileDialog();
                                        sfd.InitialDirectory = lastImportExportPath;
                                        if (sfd.ShowDialog() == DialogResult.OK)
                                        {
                                            string filename = sfd.FileName;
                                            // Save the last import/export path for later
                                            // user:
                                            try { lastImportExportPath =
                                                (new FileInfo(filename)).DirectoryName; }
                                            catch { }

                                            // Generate a generic List of SiteParameters objects.
                                            // Site Parameters are serializable, as are generic
                                            // Lists (if the underlying object is serializable), so
                                            // our output file will simply be a serialized List of
                                            // these objects, encrypted to protect their data.
                                            List<SiteParameters> siteParams =
                                                new List<SiteParameters>();
                                            foreach (object site in sitesToExport)
                                            {
                                                SiteParameters sp =
                                                    SiteParameters.ReadFromRegistry(siteParamsKey,
                                                    SiteParameters.GenerateKeyFromSite((string)site));
                                                siteParams.Add(sp);
                                            }
                                            // Serialize the List into an array of bytes:
                                            BinaryFormatter bf = new BinaryFormatter();
                                            MemoryStream ms = new MemoryStream();
                                            bf.Serialize(ms, siteParams);
                                            ms.Close();
                                            byte[] serializedParams = ms.ToArray();
                                            // Now write out those bytes in a secure form to disk:
                                            SecureFile.Write(filename, serializedParams, passphrase);
                                            MessageBox.Show("Your site parameters have been " +
                                                "successfully  exported.", "Information",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                    // The two passphrases did not match:
                                    else MessageBox.Show("The two passphrases did not match.  " +
                                            "Please try exporting your parameters again.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    // No parameters were found to export:
                    else
                    {
                        MessageBox.Show("You have no settings to export, so there's nothing " +
                           "for me to do!", "Information", MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
                        btnExport.Enabled = false;
                    }
                }
                // The registry keys weren't open:
                else MessageBox.Show("I could not open the registry to get your site parameter " +
                    "options, so I can't export anything!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Something blew up.  We need more robust, user-friendly error messages here.
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do when the Forget All button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForgetAll_Click(object sender, EventArgs e)
        {
            // Asbestos underpants:
            try
            {
                // Don't let them change their settings if they've locked the parameters:
                if (chkRemember.Checked && chkLock.Checked)
                {
                    MessageBox.Show("You currently have your site parameters locked. You " +
                        "must unlock your site parameters for this change to take effect.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnForget.Enabled = false;
                    btnForgetAll.Enabled = false;
                }
                else
                {
                    // This only makes sense if the registry is open:
                    if (CryptnosRegistryKeyOpen())
                    {
                        // And if there are any settings to forget:
                        if (siteParamsKey.GetValueNames().Length > 0)
                        {
                            // Confirm with the user that this is what they really want to do:
                            if (MessageBox.Show("This will erase all site parameter information " +
                                "currently saved in the registry.  Are you sure you'd like to " +
                                "proceed?  This procedure cannot be undone!", "Warning",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                // Delete all the site subkeys:
                                foreach (string subkey in siteParamsKey.GetValueNames())
                                    siteParamsKey.DeleteValue(subkey);
                                // Clear out the last site key:
                                CryptnosSettings.SetValue("LastSite", String.Empty,
                                    RegistryValueKind.String);
                                // Clear out the sites drop-down:
                                cbSites.Items.Clear();
                                cbSites.Text = String.Empty;
                                // Notify the user we were successful:
                                MessageBox.Show("All site parameter information has been cleared " +
                                    "from the registry.", "Information", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                // And disable the forget button for good measure"
                                btnForgetAll.Enabled = false;
                                btnForget.Enabled = false;
                            }
                        }
                        // Now settings could be found to forget:
                        else
                        {
                            MessageBox.Show("There doesn't appear to be any settings for me to " +
                               "forget.", "Information", MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                            btnForgetAll.Enabled = false;
                            btnForget.Enabled = false;
                        }
                    }
                    // the registry couldn't be opened:
                    else MessageBox.Show("I was unable to open the registry to find any settings!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // When you play with dynamite...:
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do when the Forget button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForget_Click(object sender, EventArgs e)
        {
            // Asbestos underpants
            try
            {
                // Don't let them change their settings if they've locked the parameters:
                if (chkRemember.Checked && chkLock.Checked)
                {
                    MessageBox.Show("You currently have your site parameters locked. You " +
                        "must unlock your site parameters for this change to take effect.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnForget.Enabled = false;
                    btnForgetAll.Enabled = false;
                }
                else
                {
                    // Only bother if the registry keys are open...
                    if (CryptnosRegistryKeyOpen())
                    {
                        // ... and there's something in the site box:
                        if (!String.IsNullOrEmpty(cbSites.Text))
                        {
                            // Make sure the user really, really wants to do this:
                            if (MessageBox.Show("Are you sure you would like me to forget the " +
                                "parameters for site \"" + cbSites.Text + "\"? This cannot be " +
                                "undone!", "Forget", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // Remove the site from the registry...
                                string siteName = cbSites.Text;
                                siteParamsKey.DeleteValue(SiteParameters.GenerateKeyFromSite(siteName));
                                // ... and from the sites drop-down:
                                if (cbSites.Items.IndexOf(siteName) != -1)
                                    cbSites.Items.Remove(siteName);
                                // Confirm our success:
                                MessageBox.Show("Parameters for site \"" + siteName + "\" have " +
                                    "been forgotten.", "Forget", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                        }
                        // The sites box was empty:
                        else
                        {
                            MessageBox.Show("The Site box is currently empty, so there's nothing " +
                                "for me to do!", "Warning", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                    // The registry keys weren't open:
                    else
                    {
                        MessageBox.Show("I couldn't open the PassHass registry key, so I can't " +
                            "forget the selected site!", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        btnForget.Enabled = false;
                        btnForgetAll.Enabled = false;
                        btnExport.Enabled = false;
                    }
                }
            }
            // We should really have something more user-friendly here:
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do when the Lock parameters checkbox is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {
            // If the user decides to lock the parameters, we don't want them to be able to
            // change anything, except to pick a site, enter the passphrase, generate pass-
            // phrases, and export parameters.  So disable any other UI element that would
            // change anything.
            if (chkLock.Checked)
            {
                // Make the sites combo-box a select-only drop-down:
                cbSites.DropDownStyle = ComboBoxStyle.DropDownList;
                // Disable a bunch of controls to prevent changes:
                cbHashes.Enabled = false;
                cbCharTypes.Enabled = false;
                txtCharLimit.ReadOnly = true;
                txtIterations.ReadOnly = true;
                btnForgetAll.Enabled = false;
                btnForget.Enabled = false;
                btnImport.Enabled = false;
                // This becomes a kind of extra safety valve.  To effectively forget every-
                // thing, they have to uncheck both the Lock checkbox *AND* the Remember
                // checkbox:
                chkRemember.Enabled = false;
            }
            // If they uncheck this box, enable things for editing again:
            else
            {
                // Turn the sited drop-down back into an editable combo box:
                cbSites.DropDownStyle = ComboBoxStyle.DropDown;
                // Turn back on this UI controls:
                cbHashes.Enabled = true;
                cbCharTypes.Enabled = true;
                txtCharLimit.ReadOnly = false;
                txtIterations.ReadOnly = false;
                btnImport.Enabled = true;
                // Let the uncheck the Remember box now:
                chkRemember.Enabled = true;
                // For the Forget buttons, make sure there are sites in the drop-down
                // before enabling these:
                if (cbSites.Items.Count > 0)
                {
                    btnForgetAll.Enabled = true;
                    btnForget.Enabled = true;
                }
            }
        }

        /// <summary>
        /// What to do when the ToolTips checkbox is toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowTooltips_CheckedChanged(object sender, EventArgs e)
        {
            // Fortunately, the ToolTip.Active property turns tooltips on and off, so
            // this is relatively easy:
            if (chkShowTooltips.Checked) toolTip1.Active = true;
            else toolTip1.Active = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determine if the Cryptnos registry keys are open, and open them by heck or high
        /// water if they aren't.  The <see cref="CryptnosSettings"/> and 
        /// <see cref="siteParamsKey"/> objects should be set if this is successful.  If the
        /// keys are already open, this will short-circuit and return true anyway, so it's safe
        /// to call this anywhere you want to test if the keys are open.
        /// </summary>
        /// <returns>True if the keys are currently open, false otherwise</returns>
        private bool CryptnosRegistryKeyOpen()
        {
            // Put on our asbestos underpants:
            try
            {
                // Only bother trying to open the keys if the aren't already open:
                if (CryptnosSettings == null || siteParamsKey == null)
                {
                    // HKEY_CURRENT_USER should already exist, as should the Software
                    // subkey, so this should be a safe open:
                    RegistryKey HKCU_Software =
                        Registry.CurrentUser.OpenSubKey("Software", true);
                    // However, the GPF Comics subkey may not exist unless someone is already
                    // using one of our programs.  If it doesn't exist, create it:
                    RegistryKey GPF = HKCU_Software.OpenSubKey("GPF Comics", true);
                    if (GPF == null)
                        HKCU_Software.CreateSubKey("GPF Comics");
                    // Now do the same with the Cryptnos key:
                    CryptnosSettings = GPF.OpenSubKey("Cryptnos", true);
                    if (CryptnosSettings == null)
                        CryptnosSettings = GPF.CreateSubKey("Cryptnos");
                    // Now that we've opened the Cryptnos master key, open the Sites
                    // subkey:
                    if (CryptnosSettings != null)
                    {
                        siteParamsKey = CryptnosSettings.OpenSubKey("Sites", true);
                        if (siteParamsKey == null)
                            siteParamsKey = CryptnosSettings.CreateSubKey("Sites");
                    }
                    else siteParamsKey = null;
                }
                // By this point, the registry keys are either open or not.  Return
                // that state to the caller:
                return CryptnosSettings != null && siteParamsKey != null;
            }
            // If anything blew up above, obviously the keys should not be open:
            catch { return false; }
        }

        /// <summary>
        /// Perform any necessary clean-up work to exit the application, including saving
        /// any parameter information
        /// </summary>
        private void ExitApplication()
        {
            // Only bother recording out parameter info if the registry key is available:
            if (CryptnosRegistryKeyOpen())
            {
                // If we have any settings saved and we've chosen not to remember them, we'll
                // give the user the chance to completely delete any information saved in the
                // registry.  If they say no (to keeping them) below, we'll wipe out all the
                // subkeys of the master Cryptnos key, effectively removing all the settings.
                // Otherwise, if they say yes, we'll take note of the change of the remember
                // checkbox but otherwise keep the subkeys intact.
                if (siteParamsKey.GetValueNames().Length > 0 && !chkRemember.Checked)
                {
                    if (MessageBox.Show("You have selected not to remember your site parameters.  " +
                        "This will erase any previously saved site parameters.  Are you sure " +
                        "you want to erase them?  If you say no, I'll keep them, but I won't " +
                        "populate the parameter fields next time you open this program.",
                        "Information", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Step through the subkeys and delete them all, "forgetting" all the
                        // saved parameter info:
                        foreach (string key in siteParamsKey.GetValueNames())
                            siteParamsKey.DeleteValue(key, false);
                        // Clear out the last site value so we won't try to look up a non-
                        // existant subkey the next time we load:
                        CryptnosSettings.SetValue("LastSite", String.Empty,
                            RegistryValueKind.String);
                    }
                }
                // Otherwise, if they've selected to save their info, save it for the
                // currently selected site:
                if (chkRemember.Checked && !chkLock.Checked) SaveSiteParams(cbSites.Text);
                // Always remember the state of the remember, tooltip help, and lock
                // parameters checkboxes, as well as the version info:
                CryptnosSettings.SetValue("RememberParams", (chkRemember.Checked ? 1 : 0),
                    RegistryValueKind.DWord);
                CryptnosSettings.SetValue("ToolTips", (chkShowTooltips.Checked ? 1 : 0),
                    RegistryValueKind.DWord);
                CryptnosSettings.SetValue("LockParams", (chkLock.Checked ? 1 : 0),
                    RegistryValueKind.DWord);
                CryptnosSettings.SetValue("Version",
                    Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    RegistryValueKind.String);
                // Close the Cryptnos registry keys:
                siteParamsKey.Close();
                CryptnosSettings.Close();
            }
        }

        /// <summary>
        /// Given a full site name, populate the GUI with any saved parameters for that site
        /// </summary>
        /// <param name="site">The site to look up</param>
        private void GetSiteParamsBySite(string site)
        {
            // No sense reinventing the wheel.  Generate the registry key from the site name
            // and call the GetSiteParamsByKey() method:
            GetSiteParamsByKey(SiteParameters.GenerateKeyFromSite(site));
        }

        /// <summary>
        /// Given a site registry key, populate the GUI with any saved parameters for that site
        /// </summary>
        /// <param name="key"></param>
        private void GetSiteParamsByKey(string key)
        {
            // Asbestos underpants:
            try
            {
                // If the registry is open and the supplied key isn't empty:
                if (CryptnosRegistryKeyOpen() && !String.IsNullOrEmpty(key))
                {
                    // Read the site's information from the registry.  If this happens to
                    // return null, then the site's info hasn't been saved.
                    SiteParameters siteParams =
                        SiteParameters.ReadFromRegistry(siteParamsKey, key);
                    if (siteParams != null)
                    {
                        // Character types:
                        cbCharTypes.SelectedIndex = siteParams.CharTypes;
                        // The character limit is trickier; it should be a positive integer
                        // or empty.  Since we're saving it as a number, we represent the empty
                        // value with a -1.  If we got a -1, set the text box to the empty
                        // string; otherwise, set it to the value of the key.
                        txtCharLimit.Text = siteParams.CharLimit.ToString();
                        if (txtCharLimit.Text == "-1") txtCharLimit.Text = String.Empty;
                        // If the site is in the registry, it should be in the site drop-down,
                        // so select it:
                        cbSites.SelectedItem = siteParams.Site;
                        // The hash algorithm:
                        cbHashes.SelectedItem = siteParams.Hash;
                        // The iteration count:
                        txtIterations.Text = siteParams.Iterations.ToString();
                        // Close the individual site parameters and update the last site key
                        // with the just opened value:
                        if (chkRemember.Checked)
                            CryptnosSettings.SetValue("LastSite", key, RegistryValueKind.String);
                        if (chkLock.Enabled && !chkLock.Checked)
                        {
                            btnForget.Enabled = true;
                            btnForgetAll.Enabled = true;
                        }
                    }
                }
            }
            // Silently let any errors disappear:
            catch { }
        }

        /// <summary>
        /// Populate the GUI with the parameters of the last used site
        /// </summary>
        private void GetLastSiteParams()
        {
            // Asbestos underpants:
            try
            {
                // If our registry key is open:
                if (CryptnosRegistryKeyOpen())
                {
                    // Get the last site value from the registry and populate the GUI with its
                    // parameters.  Note that the last site value could be empty or null, so
                    // we'll only bother continuing if it's not.
                    string lastSite = (string)CryptnosSettings.GetValue("LastSite", String.Empty);
                    if (!String.IsNullOrEmpty(lastSite)) GetSiteParamsByKey(lastSite);
                }
            }
            // Silently ignore any errors:
            catch { }
        }

        /// <summary>
        /// Given a site name, save its parameter information to the registry
        /// </summary>
        /// <param name="site">The site to save</param>
        private void SaveSiteParams(string site)
        {
            // Asbestos underpants:
            try
            {
                // Only bother continuing if the registry is open and the site isn't empty:
                if (CryptnosRegistryKeyOpen() && !String.IsNullOrEmpty(site))
                {
                    // Try to parse the iteration count text box value.  If it's not a positive
                    // integer greater than zero, complain.  What I'm worried about here is
                    // that I think this gets called when the application closes, so there's
                    // a chance we could lose our site params if this fails.
                    int iterations = 1;
                    bool parsedIterations = Int32.TryParse(txtIterations.Text, out iterations);
                    if (!parsedIterations || iterations < 1)
                    {
                        MessageBox.Show("The iteration count must be a positive integer " +
                            "greater than zero.", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        txtIterations.Focus();
                    }
                    else
                    {
                        // Using the GUI inputs, generate a SiteParameters object.  Note that for
                        // the character limit we'll pass in a -1 if the text box is actually
                        // empty, while the rest go in pretty much as is.
                        SiteParameters siteParams = new SiteParameters(cbSites.Text,
                            cbCharTypes.SelectedIndex, (String.IsNullOrEmpty(txtCharLimit.Text) ?
                            -1 : Int32.Parse(txtCharLimit.Text)), (string)cbHashes.SelectedItem,
                            iterations);
                        // Attempt to save the site parameters to the registry.  If that works,
                        // proceed:
                        if (siteParams.SaveToRegistry(siteParamsKey))
                        {
                            // Set the last site value to this site:
                            CryptnosSettings.SetValue("LastSite",
                                SiteParameters.GenerateKeyFromSite(cbSites.Text),
                                RegistryValueKind.String);
                            // If the site doesn't exist in the drop-down list, add it now.  That way
                            // it can be quickly selected again.
                            if (cbSites.Items.IndexOf(cbSites.Text) == -1)
                                cbSites.Items.Add(cbSites.Text);
                            btnForgetAll.Enabled = true;
                            btnExport.Enabled = true;
                            btnForget.Enabled = true;
                        }
                    }
                }
            }
            // Silently ignore errors:
            catch { }
        }

        /// <summary>
        /// Populate the sites drop-down list with any previously saved site information:
        /// </summary>
        private void PopulateSitesDropdown()
        {
            // Asbestos underpants:
            try
            {
                // Only proceed if the registry is open:
                if (CryptnosRegistryKeyOpen())
                {
                    // Clear out the current state of the list:
                    cbSites.Items.Clear();
                    // Site keys are stored in an encrypted state.  Thus, we can't use the
                    // naked values in the registry.  So, step through the list of registry
                    // key values and generate a SiteParameters object for each.  That will
                    // decrypt the values, allowing us to access the site name from the
                    // object.  Once we have the list, we'll sort it and populate the
                    // sites drop-down with the values.
                    ArrayList sites = new ArrayList();
                    foreach (string key in siteParamsKey.GetValueNames())
                    {
                        SiteParameters sp = SiteParameters.ReadFromRegistry(siteParamsKey,
                            key);
                        if (sp != null) sites.Add(sp.Site);
                    }
                    sites.Sort();
                    foreach (string site in sites) cbSites.Items.Add(site);
                    if (cbSites.Items.Count > 0)
                    {
                        btnForget.Enabled = true;
                        btnForgetAll.Enabled = true;
                        btnExport.Enabled = true;
                    }
                    else
                    {
                        btnForget.Enabled = false;
                        btnForgetAll.Enabled = false;
                        btnExport.Enabled = false;
                    }
                }
            }
            // Silently ignore errors:
            catch { }
        }

        #endregion

    }
}