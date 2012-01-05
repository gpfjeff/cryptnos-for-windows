/* Form1.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 17, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      AboutDialog, PassphraseDialog, ExportSitesForm, ImportExportHandler,
 *                UpdateChecker, IUpdateCheckListener
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
 * UPDATES FOR 1.1:  Added the "Copy password to clipboard" checkbox and code behind it.  If
 * this box is checked, the generated password is immediately copied to the system clipboard,
 * a feature first introduced in Cryptnos for Android 1.0.  This defaults to off, however,
 * which replicates the Cryptnos for Windows 1.0 functionality.  Also added the GPFUpdateChecker
 * library to let Cryptnos check the official website for updates.
 *
 * UPDATES FOR 1.2.0:  Fixed a major bug that was causing "Object reference not set to an
 * instance of an object" errros on start-up on certain systems.  Added dedicated try/catch for
 * "copy to clipboard" operation to give it a more friendly and useful error message.  Moved
 * debug mode flag out of hard-coded source to a flag in the registry.  Added Advanced settings
 * button to launch Advanced Settings dialog and let the user specify the text encoding.
 * 
 * UPDATES FOR 1.2.1:  Moved debug mode and disable update check from "undocumented" settings
 * (i.e. you have to hack the registry to enable them) to the Advanced Settings dialog.  Users
 * can now go there to turn these settings on or off.  The updates here save those settings to
 * the registry with everything else.  Also fixed a typo on the main form ("Remebering
 * Settings"); oops.
 * 
 * UPDATES FOR 1.2.2:  Replaced length restrction text box with drop-down list, which should
 * give us tighter control over the values entered as well as make the case where no restriction
 * is selected a little more clear.  Added stricter validation on the iterations text box to
 * explicitly deny non-numeric input.  Added build number to short version string for display.
 * 
 * UPDATES FOR 1.3.0:  Added "Daily Use" mode with a compact form that does not show many of
 * the UI elements unnecessary for typical daily use.  Checking and unchecking the "Enable
 * Daily Use Mode" checkbox switches back and forth between these modes.  Added "keep on top"
 * setting.  Added import dialog to allow user to pick and choose sites to import.  Added several
 * new tooltip values.
 * 
 * UPDATES FOR 1.3.1:  Added Show Master Passphrase and Clear Passwords on Focus Loss
 * functionality.
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
using com.gpfcomics.UpdateChecker;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// The main Cryptnos application form
    /// </summary>
    public partial class Form1 : Form, IUpdateCheckListener
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
            Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." +
            Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();

        /// <summary>
        /// The "generator" value to use in the new cross-platform export format
        /// </summary>
        private static string exportGenerator = "Cryptnos for Windows " +
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// The parent registry key name under which we'll create our Crypntos registry
        /// entries.  This has been pulled to the definitions at top so it can be easily
        /// changed in one place if required.
        /// </summary>
        private static string GPFRegKeyName = "GPF Comics";

        /// <summary>
        /// The Cryptnos parent registry key name.  This will be created under the key
        /// specified by <see cref="GPFRegKeyName"/> and will contain all the registry
        /// entries for the program.  This has been pulled to the definitions at top so
        /// it can be easilychanged in one place if required.
        /// </summary>
        private static string CryptnosRegKeyName = "Cryptnos";

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

        /// <summary>
        /// The encoding to use for password generation.
        /// </summary>
        private Encoding encoding = Encoding.Default;

        /// <summary>
        /// This flag helps us show the Advanced settings warning button only once per session.
        /// </summary>
        private bool showAdvancedWarning = true;

        /// <summary>
        /// A <see cref="Uri"/> for the official Cryptnos updates feed.  The
        /// <see cref="UpdateChecker"/> will use this feed to look for updated versions of
        /// Cryptnos.
        /// </summary>
        private Uri updateFeedUri =
            new Uri("http://www.cryptnos.com/files/cryptnos_updates_feed.xml");

        /// <summary>
        /// The unique app string for <see cref="UpdateChecker"/> lookups
        /// </summary>
        private string updateFeedAppName = "Cryptnos for Windows";

        /// <summary>
        /// The last update check timestamp for <see cref="UpdateChecker"/> lookups.  Note
        /// that this defaults to <see cref="DateTime"/>.MinValue, which should force an
        /// update on the first check, but that will be overwritten during initialization.
        /// </summary>
        private DateTime updateFeedLastCheck = DateTime.MinValue;

        /// <summary>
        /// The actual <see cref="UpdateChecker"/> object, which will check for Cryptnos
        /// updates
        /// </summary>
        private UpdateChecker.UpdateChecker updateChecker = null;

        /// <summary>
        /// This Boolean flag sets the program to be in full debug mode, showing much more
        /// detailed error messages than if debugging mode is turned off.  This should be
        /// set to false for official releases.
        /// </summary>
        private bool debug = false;

        /// <summary>
        /// This Boolean flag determines whether or not to disable the built-in check for
        /// updates.  This isn't recommended, of course, but a feature nonetheless.
        /// </summary>
        private bool disableUpdateCheck = false;

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
        /// This flag gets set to true if this is the very first time Cryptnos has been
        /// run for this user.
        /// </summary>
        private bool veryFirstTime = false;

        /// <summary>
        /// This <see cref="Size"/> represents the size of the main form in its fully
        /// expanded mode.
        /// </summary>
        private Size sizeFullForm = new Size(298, 449);

        /// <summary>
        /// This <see cref="Size"/> represents the size of the main form in its compact
        /// "daily use" mode.
        /// </summary>
        private Size sizeDailyUse = new Size(298, 187);

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            try
            {
                // Begin with the default initialization:
                InitializeComponent();
                // Default to using all the available characters in the hash:
                cbCharTypes.SelectedIndex = 0;
                // Originally we bound the hash drop-down to the HashEngine Hashes enum, but that
                // no longer works with our cross-platform compatibility code.  Instead, step
                // through the enumeration and convert its value to the "display" value, using
                // the handy method provided by HashEngine.
                foreach (Hashes item in Enum.GetValues(typeof(Hashes)))
                {
                    cbHashes.Items.Add(HashEngine.HashEnumToDisplayHash(item));
                }
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
                // Default the hash selection to SHA-1.  We probably ought to default this to
                // something stronger eventually.
                cbHashes.SelectedItem = HashEngine.HashEnumToDisplayHash(Hashes.SHA1);
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
                        // Get the user's copy to clipboard preference:
                        chkCopyToClipboard.Checked = (int)CryptnosSettings.GetValue("CopyToClipboard", 0)
                            == 1 ? true : false;
                        // Get the last update check date.  I'm not sure if the try/catch block is
                        // really necessary, but I'm a belt-and-suspenders guy.  Also note that
                        // the default, whether the parse fails for the registry value isn't set,
                        // is DateTime.MinValue, which pretty much guarantees an update check on
                        // the first go-around.
                        try
                        {
                            updateFeedLastCheck =
                                DateTime.Parse((string)CryptnosSettings.GetValue("LastUpdateCheck",
                                DateTime.MinValue.ToString()));
                        }
                        catch { updateFeedLastCheck = DateTime.MinValue; }
                        // Get the "daily mode" flag:
                        chkDailyMode.Checked = (int)CryptnosSettings.GetValue("DailyMode", 0)
                            == 1 ? true : false;
                        this.TopMost = (int)CryptnosSettings.GetValue("KeepOnTop", 0)
                            == 1 ? true : false;
                        // Get the disable update check flag.  This used to be an "undocumented"
                        // feature with no user interface option but I felt giving the user the
                        // choice is always better than not.  Note that the default is false,
                        // meaning we will *not* disable the check by default.
                        disableUpdateCheck = (int)CryptnosSettings.GetValue("DisableUpdateCheck",
                            0) == 1 ? true : false;
                        // Turn debug mode on or off:
                        debug = (int)CryptnosSettings.GetValue("DebugMode",
                            0) == 1 ? true : false;
                        // Get the user's preference on whether or not the master passphrase should
                        // be visible or not.  It should be obscured by default.
                        showMasterPassword = (int)CryptnosSettings.GetValue("ShowMasterPassword",
                            0) == 1 ? true : false;
                        txtPassphrase.UseSystemPasswordChar = !showMasterPassword;
                        // Get the user's preference of whether or not the master passphrase and
                        // generated password should be cleared when Cryptnos loses focus:
                        clearPasswordsOnFocusLoss = (int)CryptnosSettings.GetValue("ClearPasswordsOnFocusLoss",
                            0) == 1 ? true : false;
                        // Get the encoding.  Note the funky way we'll try to set the default.
                        // If this is the very first time the user has tried to run Cryptnos
                        // (determined within CryptnosRegistryKeyOpen()), we'll try to default
                        // the user to UTF-8.  However, if they've run the app before, we'll
                        // use the system default, which is the old behavior.  Then we'll pass
                        // this "default" as the default to the registry check.  Thus, if the
                        // user has already set an encoding preference, that takes precedence.
                        // If there's no user preference, use the "default" we set depending
                        // on whether or not the user has run Cryptnos before.  This same
                        // process goes for the catch block below.
                        Encoding defaultEncoding = Encoding.Default;
                        if (veryFirstTime) defaultEncoding = Encoding.UTF8;
                        encoding = Encoding.GetEncoding((string)CryptnosSettings.GetValue("Encoding",
                            defaultEncoding.WebName));
                    }
                }
                // If anything above blew up, set some sensible defaults:
                catch
                {
                    chkDailyMode.Checked = false;
                    chkRemember.Checked = true;
                    chkLock.Checked = false;
                    cbCharTypes.SelectedIndex = 0;
                    cbSites.Text = String.Empty;
                    txtPassphrase.Text = String.Empty;
                    txtIterations.Text = "1";
                    cbHashes.SelectedItem = HashEngine.HashEnumToDisplayHash(Hashes.SHA1);
                    cbCharLimit.SelectedIndex = 0;
                    btnForget.Enabled = false;
                    btnForgetAll.Enabled = false;
                    btnExport.Enabled = false;
                    chkShowTooltips.Checked = true;
                    chkCopyToClipboard.Checked = false;
                    updateFeedLastCheck = DateTime.MinValue;
                    if (veryFirstTime) encoding = Encoding.UTF8;
                    else encoding = Encoding.Default;
                    TopMost = false;
                }
                // Turn on or off tooltip help depending on the user's save preference:
                toolTip1.Active = chkShowTooltips.Checked;
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
                try
                {
                    lastImportExportPath =
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                catch { lastImportExportPath = Environment.CurrentDirectory; }

                // Finally, initialize the update checker and set it to work.  The update check
                // should occur in a separate thread, which will allow the main UI thread to
                // continue without any problems.  The entire process *should* be transparent to
                // the user unless an update is actually found.
                if (!disableUpdateCheck)
                {
                    updateChecker = new UpdateChecker.UpdateChecker(updateFeedUri, updateFeedAppName,
                        Assembly.GetExecutingAssembly().GetName().Version, this, updateFeedLastCheck);
                    updateChecker.CheckForNewVersion();
                }
            }
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                else MessageBox.Show("I encountered an error while trying to launch " +
                    "Cryptnos. Please close the application and try to restart it.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                Hashes hashAlgo =
                    HashEngine.DisplayHashToHashEnum((string)cbHashes.SelectedItem);
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
                // If all those are OK, move on to the actual work:
                else
                {
                    // Generate the hash.  For the text, combine the site name with
                    // the passphrase text to give us a unique plaintext input.  Note that
                    // the output is Base64 to give us a healthy output.
                    string hash = HashEngine.HashString(hashAlgo, encoding,
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
                    if (cbCharLimit.SelectedIndex != 0)
                        hash = hash.Substring(0, cbCharLimit.SelectedIndex);
                    // Now that the hash has been generated and tweaked, display it in the
                    // password box:
                    txtPassword.Text = hash;
                    // Copy the value of the hash to the system clipboard:
                    try { if (chkCopyToClipboard.Checked) Clipboard.SetText(hash); }
                    // For some reason, copying stuff to the clipboard doesn't always work.
                    // This may be a known problem with Windows itself, but there's no consensus
                    // on the issue.  Anyway, we don't want the entire process to bomb on this
                    // trivial little issue, so we'll catch it separately, inform the user that
                    // we couldn't copy it, and ask them to do it themselves.
                    catch
                    {
                        MessageBox.Show("I was unable to copy the generated password to " +
                            "the clipboard. This is usually a transient problem within " +
                            "Windows itself that does not occur regularly. Please copy your " +
                            "password manually this time.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // If the user has elected to save their settings, do so now:
                    if (chkRemember.Checked && !chkLock.Checked) SaveSiteParams(cbSites.Text);
                }
            }
            // If any errors occur, show an error message.  We should really make this more
            // robust and user-friendly.
            catch (Exception ex)
            {
                if (debug)
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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
        /// What to do when the form loses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            // If the user prefers to clear the password boxes when the form loses focus,
            // do so now:
            if (clearPasswordsOnFocusLoss)
            {
                txtPassphrase.Text = "";
                txtPassword.Text = "";
            }
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
                // Re-enable the Lock and Daily Mode checkboxes:
                chkLock.Enabled = true;
                chkDailyMode.Enabled = true;
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
                chkDailyMode.Enabled = false;
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
                chkShowTooltips.Checked, TopMost);
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
                // This should never happen as the import button gets disabled when the Lock checkbox
                // is checked, but as a sanity check, don't let them change their settings if they've
                // locked the parameters:
                if (chkRemember.Checked && chkLock.Checked)
                {
                    MessageBox.Show("You currently have your site parameters locked. You " +
                        "must unlock your site parameters for this change to take effect.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnForget.Enabled = false;
                    btnForgetAll.Enabled = false;
                }
                // Otherwise:
                else
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
                            // import/export path for later use.  Note that if, for some
                            // bizarre reason, the last import/export path fetch fails,
                            // we don't sweat it.  That's not important right now.
                            string filename = ofd.FileName;
                            try
                            {
                                lastImportExportPath =
                                    (new FileInfo(filename)).DirectoryName;
                            }
                            catch { }
                            // Prompt the user for their passphrase (Cryptnos files are always
                            // encrypted):
                            PassphraseDialog pd =
                                new PassphraseDialog(PassphraseDialog.Mode.Import, TopMost);
                            if (pd.ShowDialog() == DialogResult.OK)
                            {
                                string passphrase = pd.Passphrase;
                                pd.Dispose();
                                // Read the data from the import file.  For this, we hand
                                // the heavy lifting to the ImportExportHandler.  Note that
                                // this should transparently handle both the old and new
                                // export formats, so we don't have to sweat that detail.
                                List<SiteParameters> siteParamList =
                                    ImportExportHandler.ImportFromFile(filename, passphrase);
                                if (siteParamList != null && siteParamList.Count > 0)
                                {
                                    // Launch the import dialog so the user can pick and choose which
                                    // sites to import:
                                    ImportDialog id = new ImportDialog(this, siteParamList, debug, TopMost,
                                        chkShowTooltips.Checked);
                                    if (id.ShowDialog() == DialogResult.OK)
                                    {
                                        // Get the filtered list of parameters.  Note that this should
                                        // always have something in it; if the list were empty, the dialog
                                        // should switch to a Cancel result.
                                        siteParamList = id.SiteParams;
                                        // If the import didn't blow up, step through the list
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
                                // I don't think this will happen (an exception should be thrown if the file
                                // is invalid), but if the site list is empty, complain:
                                else MessageBox.Show("No valid Cryptnos parameters could be loaded " +
                                    "from the file. Make sure the file is not corrupted", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    // The registry keys weren't open:
                    else MessageBox.Show("I could not open the registry to get your site parameter " +
                        "options, so I can't import anything!", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            // If we got an ImportHandlerException, it probably has some sort of very specific
            // message we can show back to the user:
            catch (ImportHandlerException ihe)
            {
                MessageBox.Show(ihe.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            // Otherwise, if something else blew up, notify the user.  This needs to be more
            // robust with more specific and user-friendly messages.
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
                            chkShowTooltips.Checked, TopMost, this);
                        // If they selected anything to export:
                        if (esf.ShowDialog() == DialogResult.OK && esf.SelectedSites != null
                            && esf.SelectedSites.Length > 0)
                        {
                            sitesToExport = esf.SelectedSites;
                            // Prompt the user for a passphrase.  All Cryptnos export files are
                            // encrypted, so we need a passphrase to encrypt the data with.
                            PassphraseDialog pd =
                                new PassphraseDialog(PassphraseDialog.Mode.Export_Initial, TopMost);
                            if (pd.ShowDialog() == DialogResult.OK)
                            {
                                string passphrase = pd.Passphrase;
                                pd.Dispose();
                                // Now prompt them for their passphrase again to make sure they
                                // don't misstype it:
                                pd = new PassphraseDialog(PassphraseDialog.Mode.Export_Confirm, TopMost);
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
                                            // use.  Note that if this fails, don't sweat it.
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
                                            // Hand the actual heavy lifting off to the
                                            // import/export handler class:
                                            ImportExportHandler.ExportToFile(filename,
                                                passphrase, exportGenerator, siteParams);
                                            // If we get this far, we must have been successful.
                                            // Display a success message to the user:
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
                if (debug)
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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
                if (debug)
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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
                if (debug)
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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
                cbCharLimit.Enabled = false;
                txtIterations.Enabled = false;
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
                cbCharLimit.Enabled = true;
                txtIterations.Enabled = true;
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

        /// <summary>
        /// What to do when the text in the iterations box changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtIterations_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // We're going to be a little more brute-force with this text box than we
                // used to be.  We're going to absolutely deny input of any non-numeric
                // characters.  So the first thing we'll do is use a regular expression to
                // eliminate any non-numeric characters right off the bat.
                txtIterations.Text = Regex.Replace(txtIterations.Text, @"\D", "");
                // Also eliminate any leading zeros that might be present:
                txtIterations.Text = Regex.Replace(txtIterations.Text, @"^0+", "");
                // Of course, a caveat of the above is that we now might end up with an
                // empty field, which is also unacceptable.  If that occurs, set the value
                // of the box back to the default, which is one.
                if (String.IsNullOrEmpty(txtIterations.Text)) txtIterations.Text = "1";
                // After those steps, this should never fail to parse, but we'll keep it in
                // the try/catch just to make sure.  Try to parse the value of the iterations
                // box as an integer:
                int iters = Int32.Parse(txtIterations.Text);
                // If that works, make sure the value isn't less than one.  We need at least
                // one iteration to be performed, so anything less won't work.
                if (iters < 1)
                {
                    MessageBox.Show("The number of hash iterations must be a positive integer " +
                        "greater than zero, with a minimum of 1. Values greater than 500 are " +
                        "discouraged because they may not be compatible with versions of Cryptnos " +
                        "on other platforms.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtIterations.Focus();
                    txtIterations.SelectAll();
                }
                // Similarly, test to see if the value is over 500.  That's the maximum number
                // of iterations currently set for Cryptnos for Android.  Anything higher than
                // that causes serious performance issues on Android, so we cap this value
                // there.  Windows machines likely have a lot more power than Android phones,
                // and who knows, there might be folks out there actually using values this
                // high.  If they are, warn them that this may not be supported on other
                // platforms, but don't force them to change it.  We don't want to break things
                // for folks who actually are using it this way.
                else if (iters > 500)
                    MessageBox.Show("Hash iteration values greater than 500 are discouraged " +
                        "because they may not be compatible with versions of Cryptnos on " +
                        "other platforms.", "Warning", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
            }
            // This may not be the most efficient way of doing this, but if the user enters
            // anything other than an integer, the parse above will fail.  Complain and make
            // them change it here.
            catch
            {
                MessageBox.Show("The number of hash iterations must be a positive integer " +
                    "greater than zero, with a minimum of 1. Values greater than 500 are " +
                    "discouraged because they may not be compatible with versions of Cryptnos " +
                    "on other platforms.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtIterations.Focus();
                txtIterations.SelectAll();
            }
        }

        /// <summary>
        /// What to do when the Advanced button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            // Start off by declaring the advanced settings dialog, but don't initialize
            // it yet.  We'll get to that in a bit.
            AdvancedSettingsDialog ads = null;
            // The first time the user attempts to access the advanced settings, show them
            // a warning box letting them know they could be setting themselves up for
            // trouble if they go mucking around with this.  Once they click through,
            // don't bother again until the next session.
            if (showAdvancedWarning)
            {
                if (MessageBox.Show("Modifying the advanced settings of Cryptnos may " +
                    "\"break\" your existing passwords or result in unexpected or undesired " +
                    "behavior. Are you sure you wish to continue?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    showAdvancedWarning = false;
                    ads = new AdvancedSettingsDialog(encoding, chkShowTooltips.Checked, debug,
                        disableUpdateCheck, this.TopMost, showMasterPassword, clearPasswordsOnFocusLoss); 
                }
            }
            else ads = new AdvancedSettingsDialog(encoding, chkShowTooltips.Checked, debug,
                disableUpdateCheck, this.TopMost, showMasterPassword, clearPasswordsOnFocusLoss);
            // Now if the user got to here and the dialog was created, go ahead and
            // show it.  If they click OK, get the new settings and take note of
            // them.  We'll go ahead and write the values into the registry too.
            if (ads != null)
            {
                if (ads.ShowDialog() == DialogResult.OK)
                {
                    encoding = ads.Encoding;
                    debug = ads.Debug;
                    disableUpdateCheck = ads.DisableUpdateCheck;
                    this.TopMost = ads.KeepOnTop;
                    showMasterPassword = ads.ShowMasterPassword;
                    clearPasswordsOnFocusLoss = ads.ClearPasswordsOnFocusLoss;
                    // If the user did not disable the update check and they requested that
                    // we force an update check the next time we launch, set the last update
                    // check date/time to the minimum value of DateTime, which will be well
                    // outside the expiration window and which will force an update check the
                    // next time we launch.
                    if (!disableUpdateCheck && ads.ForceUpdateCheck)
                        updateFeedLastCheck = DateTime.MinValue;
                    // Tweak the master password box based on the new Show Master Passphrase
                    // preference:
                    txtPassphrase.UseSystemPasswordChar = !showMasterPassword;
                    if (CryptnosRegistryKeyOpen())
                    {
                        CryptnosSettings.SetValue("Encoding", encoding.WebName,
                            RegistryValueKind.String);
                        CryptnosSettings.SetValue("DisableUpdateCheck",
                            (disableUpdateCheck ? 1 : 0), RegistryValueKind.DWord);
                        CryptnosSettings.SetValue("DebugMode", (debug ? 1 : 0),
                            RegistryValueKind.DWord);
                        CryptnosSettings.SetValue("LastUpdateCheck",
                            updateFeedLastCheck.ToString(), RegistryValueKind.String);
                        CryptnosSettings.SetValue("ShowMasterPassword", (showMasterPassword ? 1 : 0),
                            RegistryValueKind.DWord);
                        CryptnosSettings.SetValue("ClearPasswordsOnFocusLoss", (clearPasswordsOnFocusLoss ? 1 : 0),
                            RegistryValueKind.DWord);
                    }
                }
            }
        }

        /// <summary>
        /// What to do when the hash combo box is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbHashes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the newly selected hash algorithm, and from there get the new
            // maximum hash length:
            Hashes newHashAlgo =
                    HashEngine.DisplayHashToHashEnum((string)cbHashes.SelectedItem);
            int newHashLength = (int)hashLengths[newHashAlgo];
            // Next, get the originally selected old character limit, which for our
            // purposes just has to be the selected index of the character limit
            // drop-down:
            int oldCharLimit = cbCharLimit.SelectedIndex;
            // Clear out the character limit drop-down item list and add back in the
            // default "None" case:
            cbCharLimit.Items.Clear();
            cbCharLimit.Items.Add("None");
            // Now add back in the other limit values, looping from one to the maximum
            // length of the hash.  Note that by happy coincidence we now can use the
            // selection index directly to get the character limit (with the exception of
            // the "None" case, which we'll have to test for specifically).
            for (int i = 1; i <= newHashLength; i++) cbCharLimit.Items.Add(i);
            // Now to set our current selection.  If the old selection exceeds the new
            // maximum, we might as well set the new selection to "None".  Otherwise,
            // preserve the user's old preference.
            if (oldCharLimit > newHashLength) cbCharLimit.SelectedIndex = 0;
            else cbCharLimit.SelectedIndex = oldCharLimit;
        }

        /// <summary>
        /// What to do when the "Enable daily use mode" checkbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDailyMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDailyMode.Checked)
            {
                // Force us to lock all parameters so they can't be changed:
                if (!chkLock.Checked) chkLock.Checked = true;
                // Hide the elements we don't need:
                gboxOptionalRules.Visible = false;
                gboxRememberingSettings.Visible = false;
                chkShowTooltips.Visible = false;
                chkCopyToClipboard.Visible = false;
                btnAbout.Visible = false;
                btnAdvanced.Visible = false;
                btnClose.Visible = false;
                // Resize the form to its compact size:
                this.Size = sizeDailyUse;
            }
            else
            {
                // Resize the form to its full size:
                this.Size = sizeFullForm;
                // Show the hidden form elements:
                gboxOptionalRules.Visible = true;
                gboxRememberingSettings.Visible = true;
                chkShowTooltips.Visible = true;
                chkCopyToClipboard.Visible = true;
                btnAbout.Visible = true;
                btnAdvanced.Visible = true;
                btnClose.Visible = true;
            }
        }

        #endregion

        #region Interoperation Methods

        /// <summary>
        /// Get the site parameters for a site.  This is intended to be used by
        /// the QRCode export generator.
        /// </summary>
        /// <param name="siteName">The site name <see cref="String"/></param>
        /// <returns>A <see cref="SiteParameters"/> object representing the site</returns>
        public SiteParameters GetSiteParamsForQRCode(String siteName)
        {
            try
            {
                // Convert the site name to a registry key:
                String key = SiteParameters.GenerateKeyFromSite(siteName);
                // If the registry is open and the supplied key isn't empty:
                if (CryptnosRegistryKeyOpen() && !String.IsNullOrEmpty(key))
                {
                    // Read the site's information from the registry.  If this happens to
                    // return null, then the site's info hasn't been saved.
                    return SiteParameters.ReadFromRegistry(siteParamsKey, key);
                }
                // If the registry isn't open or the key was useless, return null:
                else return null;
            }
            // If anything blew up, return null:
            catch { return null; }
        }

        /// <summary>
        /// Get the list of site names stored in the registry
        /// </summary>
        /// <returns>An array of <see cref="Stirng"/>s containing the list of site names</returns>
        public string[] GetSiteList()
        {
            // Asbestos underpants:
            try
            {
                // Only proceed if the registry is open:
                if (CryptnosRegistryKeyOpen())
                {
                    // Site keys are stored in an encrypted state.  Thus, we can't use the
                    // naked values in the registry.  So, step through the list of registry
                    // key values and generate a SiteParameters object for each.  That will
                    // decrypt the values, allowing us to access the site name from the
                    // object.
                    ArrayList sites = new ArrayList();
                    foreach (string key in siteParamsKey.GetValueNames())
                    {
                        SiteParameters sp = SiteParameters.ReadFromRegistry(siteParamsKey,
                            key);
                        if (sp != null) sites.Add(sp.Site);
                    }
                    // Sort the list:
                    sites.Sort();
                    // I'm not sure if there's a better way to do this or not, be we need to
                    // convert the ArrayList into simple string array.  We'll do this site by
                    // site.  It would be nice if we could use ArrayList.ToArray() for this,
                    // but I don't think it works the way I want it to.
                    string[] siteArray = new string[sites.Count];
                    int i = 0;
                    foreach (string site in sites)
                    {
                        siteArray[i] = site;
                        i++;
                    }
                    return siteArray;
                }
                // If the registry isn't open, there's nothing to return:
                else return null;
            }
            // Silently ignore errors:
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return null;
            }
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
                    RegistryKey GPF = HKCU_Software.OpenSubKey(GPFRegKeyName, true);
                    if (GPF == null)
                        GPF = HKCU_Software.CreateSubKey(GPFRegKeyName);
                    // Now do the same with the Cryptnos key:
                    CryptnosSettings = GPF.OpenSubKey(CryptnosRegKeyName, true);
                    if (CryptnosSettings == null)
                    {
                        CryptnosSettings = GPF.CreateSubKey(CryptnosRegKeyName);
                        // If the Cryptnos registry has never been created, this is likely
                        // the very first time the user has ever run Cryptnos.  In that
                        // case, we want to take note of that so we can set some defaults
                        // later.
                        veryFirstTime = true;
                    }
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
            catch (Exception ex) {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Perform any necessary clean-up work to exit the application, including saving
        /// any parameter information
        /// </summary>
        private void ExitApplication()
        {
            try
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
                    // Store the rest of our settings.  Note that booleans are saved as DWORDs
                    // while the others are saved as strings.  The version information is used
                    // for the "downgrade" warning when the program starts.
                    CryptnosSettings.SetValue("DailyMode", (chkDailyMode.Checked ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("KeepOnTop", (TopMost ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("RememberParams", (chkRemember.Checked ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("ToolTips", (chkShowTooltips.Checked ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("LockParams", (chkLock.Checked ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("CopyToClipboard", (chkCopyToClipboard.Checked ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("LastUpdateCheck", updateFeedLastCheck.ToString(),
                        RegistryValueKind.String);
                    CryptnosSettings.SetValue("DisableUpdateCheck", (disableUpdateCheck ? 1: 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("ShowMasterPassword", (showMasterPassword ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("ClearPasswordsOnFocusLoss", (clearPasswordsOnFocusLoss ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("DebugMode", (debug ? 1 : 0),
                        RegistryValueKind.DWord);
                    CryptnosSettings.SetValue("Encoding", encoding.WebName,
                        RegistryValueKind.String);
                    CryptnosSettings.SetValue("Version",
                        Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                        RegistryValueKind.String);
                    // Close the Cryptnos registry keys:
                    siteParamsKey.Close();
                    CryptnosSettings.Close();
                }
            }
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                        // The hash algorithm:
                        cbHashes.SelectedItem =
                            HashEngine.HashEnumStringToDisplayHash(siteParams.Hash);
                        // If the site is in the registry, it should be in the site drop-down,
                        // so select it:
                        cbSites.SelectedItem = siteParams.Site;
                        // The character limit is trickier.  It should either be a positive
                        // integer (indicating a limit is imposed) or -1 (indicating no limit).
                        // we used to get this from a text box, but now we use a drop-down list
                        // with the first item (index zero) being "None".  This now means we can
                        // use the index of the drop-down directly for the character limit,
                        // except for the case where "None" is selected.  In that case, we need
                        // to convert the -1 from the parameters object into a 0 for the
                        // drop-down.
                        if (siteParams.CharLimit < 0) cbCharLimit.SelectedIndex = 0;
                        else cbCharLimit.SelectedIndex = siteParams.CharLimit;
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
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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
                        // the character limit we'll pass in a -1 if the drop-down is actually
                        // set to the first item ("None"), while the rest go in pretty much as
                        // is.
                        SiteParameters siteParams = new SiteParameters(cbSites.Text,
                            cbCharTypes.SelectedIndex,
                            cbCharLimit.SelectedIndex == 0 ? -1 : cbCharLimit.SelectedIndex,
                            HashEngine.DisplayHashToHashEnumString((string)cbHashes.SelectedItem),
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
            // Clear out the current state of the list:
            cbSites.Items.Clear();
            // Get the list of sites from the registry:
            string[] sites = GetSiteList();
            // If there are any sites to work with, populate the sites drop-down:
            if (sites != null)
                foreach (string site in sites) cbSites.Items.Add(site);
            // Enable or disable other GUI elements based on whether or not we got any
            // sites:
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

        #endregion

        #region IUpdateCheckListener Members

        // These methods implement the IUpdateCheckListener interface, which is used to check
        // for updates for Cryptnos.  They shouldn't have to actually do much, as the update
        // checker does most of the work.

        /// <summary>
        /// What to do if a new update is found.
        /// </summary>
        public void OnFoundNewerVersion()
        {
            // This is pretty simple.  If the update check found a new version, tell it to
            // go ahead and download it.  Note that the update checker will handle any user
            // notifications, which includes a prompt on whether or not they'd like to
            // upgrade.  The null check is probably redudant--this method should never be
            // called if the update checker is null--but it's a belt-and-suspenders thing.
            try { if (updateChecker != null) updateChecker.GetNewerVersion(); }
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do if the update checker says to record a new last update check date.
        /// This gets called by the update checker whenever a check is started, whether it
        /// is successful or not.
        /// </summary>
        /// <param name="lastCheck">The new date of the last update check</param>
        public void OnRecordLastUpdateCheck(DateTime lastCheck)
        {
            // Update the last update check date, both within our private copy in memory
            // and in the registry:
            try
            {
                updateFeedLastCheck = lastCheck;
                if (CryptnosRegistryKeyOpen())
                    CryptnosSettings.SetValue("LastUpdateCheck", updateFeedLastCheck.ToString(),
                        RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                if (debug) MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// What to do if the update checker wants us to close.  This gets called if the
        /// update check has successfully download the file and now wants to install the
        /// new version.
        /// </summary>
        public void OnRequestGracefulClose()
        {
            // We don't have a lot to do to close up shop.  Fortunately, we already have
            // a method to do all that stuff, so call it:
            ExitApplication();
        }

        #endregion

    }
}