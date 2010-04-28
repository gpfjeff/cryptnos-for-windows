/* ImportExportHandler.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          April 28, 2010
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      See using statements
 * REQUIRED BY:   Form1
 * 
 * This static class provides the implementation of the Cryptnos import/export mechanism.  It
 * essentially exposes two basic methods:  ImportFromFile(), which reads a list of
 * SiteParameters objects from the file specified; and ExportToFile(), which writes a list of
 * SiteParameters to a file.  Both require two strings, one containing the full path to the
 * file to import or export and the other containing the encryption/decryption password.  They
 * also throw a number of Exception classes, so make sure these are caught in any code that
 * uses them.
 * 
 * Cryptnos for Windows supports two different export formats.  Originally, every version of
 * Cryptnos exported parameters in a platform-specific format, meaning you could not export
 * your parameters with the Windows version and then import it using the Java or Google Android
 * versions.  Reading this format is preserved by ImportFromFile(), so old export files can
 * still be used.  However, a new cross-platform export format has been developed using XML
 * as a base, which is subsequently compressed (using gzip) and then encrypted using a password-
 * based AES-256 cipher.  This format can be read by every version of Cryptnos going forward.
 * Note that this ImportExportHandler class can only export parameters in the new, cross-
 * platform format; it will not export using the old platform-specific format.  This will
 * hopefully increase compatibility with the various versions of Cryptnos over time.  The
 * ImportFromFile() method transparently handles the format of the file being formatted, so the
 * implementor need not worry about that detail.
 * 
 * While both public methods throw exceptions, ImportFromFile() may throw an
 * ImportHandlerException in specific.  This exception contains specific messages describing
 * parsing or encryption errors while importing a file.  When using ImportExportHandler, make
 * sure to catch these exceptions first and then handle any other exception after that.
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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Digests;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// This static class implements methods to import and export encrypted site parameter
    /// files for Cryptnos.  Currently, these parameter files come in two formats:  an older,
    /// platform specific flavor that was made for the original 1.0 release, and a newer,
    /// cross-platform compatible version for 1.1 onwards.  This class should transparently
    /// choose between these two while importing, but will only export in the newer cross-
    /// platform format.
    /// </summary>
    static public class ImportExportHandler
    {
        #region Private Constants

        /// <summary>
        /// The number of iterations used for salt generation.  For the encryption
        /// used in this class, we'll derive our salt from the user's password;
        /// not ideal, of course, but definitely portable.  This constant will set
        /// the number of times we'll hash the user's password with the selected
        /// hash algorithm to generate our salt.
        /// </summary>
        private const int SALT_ITERATION_COUNT = 10;

        /// <summary>
        /// The number of iterations used for key generation.
        /// </summary>
        private const int KEY_ITERATION_COUNT = 100;

        /// <summary>
        /// The size of the AES encryption key in bits
        /// </summary>
        private const int KEY_SIZE = 256;
        
        /// <summary>
        /// The size of the AES encryption intialization vector (IV) in bits
        /// </summary>
        private const int IV_SIZE = 128;

        #endregion

        #region Public Methods

        /// <summary>
        /// Read a <see cref="List"/> of <see cref="SiteParameters"/> from an encrypted export
        /// file.  This method should distinguish between the various different versions of
        /// export files and correctly read the right one.
        /// </summary>
        /// <param name="filename">A string containing the full path to the import file</param>
        /// <param name="password">A string containing the password used to decrypt the
        /// file</param>
        /// <returns>A <see cref="List"/> of <see cref="SiteParameters"/></returns>
        /// <exception cref="ArgumentException">Thrown if the password is empty or
        /// null</exception>
        /// <exception cref="FileNotFoundException">Thrown if the import file could not be
        /// found</exception>
        /// <exception cref="ImportHandlerException">Thrown if the XML-based export file is
        /// invalid in so way</exception>
        /// <exception cref="Exception">Thrown if anything else blows up along the
        /// way</exception>
        public static List<SiteParameters> ImportFromFile(string filename, string password)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                // Make sure the file actually exists:
                if (File.Exists(filename))
                {
                    // Make sure the password is not null or empty:
                    if (!String.IsNullOrEmpty(password))
                    {
                        // This is probably horribly inefficient, but we'll do this the brute
                        // force method and try reading the file as an XML parameter file first.
                        // If that blows up, then we'll try to do it as an original platform-
                        // specific export file.
                        try { return ImportFromXMLv1File(filename, password); }
                        // If we the above threw a specific exception type, it was a parsing
                        // error, so the file was invalid.  In this case, we shouldn't try to
                        // read the old format; we should just throw up our hands and give up.
                        catch (ImportHandlerException ihe) { throw ihe; }
                        // If we got any other type of exception, go ahead and try to read the
                        // file as an original format file.  If *this* blows up, then the file
                        // must be bad to begin with.
                        catch (Exception xmlEx)
                        {
                            try { return ImportFromOriginalExportFile(filename, password); }
                            catch (Exception origEx) { throw origEx; }
                        }
                    }
                    // The password was empty or null:
                    else throw new ArgumentException("Password is empty or null.");
                }
                // The file could not be found:
                else throw new FileNotFoundException();
            }
            // The file name was empty or null:
            else throw new ArgumentException("File name is empty or null.");
        }

        /// <summary>
        /// Export a list of <see cref="SiteParameters"/> to an encrypted export file.  Note
        /// that this method only exports to the newer XML-based cross-platform format, not
        /// the old platform specific format that is no longer supported.
        /// </summary>
        /// <param name="filename">A string containing the full path to the export file</param>
        /// <param name="password">A string containing the password used to encrypt the
        /// file</param>
        /// <param name="generator">A string containing the "generator" ID for the file, usually
        /// the full application name ("Cryptnos for Windows") and version number.  If this
        /// value is null the generator tag will be omitted from the file.</param>
        /// <param name="comment">A string containing an optional comment.    If this value is
        /// null the comment tag will be omitted from the file.</param>
        /// <param name="siteList">A <see cref="List"/> of <see cref="SiteParameters"/> to
        /// export</param>
        /// <exception cref="ArgumentException">Thrown if any required field (file name,
        /// password, or site list) is empty or null</exception>
        /// <exception cref="Exception">Thrown if anything blows up along the way</exception>
        public static void ExportToFile(string filename, string password, string generator,
            string comment, List<SiteParameters> siteList)
        {
            try
            {
                // A little bit of sanity checking.  Make sure our required inputs are
                // not null or empty:
                if (String.IsNullOrEmpty(filename))
                    throw new ArgumentException("File name is empty or null");
                if (String.IsNullOrEmpty(password))
                    throw new ArgumentException("Password is empty or null");
                if (siteList == null || siteList.Count == 0)
                    throw new ArgumentException("Site parameter list is empty or null");

                // Set up the XML formatting options for our XML writer.  I don't think these
                // guys are actually essential, given that our XML is not meant to be human
                // readable, but they seemed to work for Mandelbrot Madness! so I'll keep them
                // here.
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.IndentChars = "\t";
                xws.CloseOutput = true;
                // We won't be writing directly to a file, at least not yet.  Create a memory
                // stream for us to write to initially, then open up the XML writer to point
                // to that stream.  Note that we'll also gzip the XML as it goes into the
                // memory stream to compress it.
                MemoryStream ms = new MemoryStream();
                XmlWriter xw = XmlWriter.Create(new GZipStream(ms,
                    CompressionMode.Compress), xws);
                // Start writing out our XML by putting in the required headers.  Note that
                // the <version> tag is required and for now must be 1, but the <generator>
                // and <comment> tags are technically optional.  Generator is highly recommended,
                // however, as that helps us ID where the file came from.
                xw.WriteStartDocument();
                xw.WriteStartElement("cryptnos", "http://www.cryptnos.com/");
                xw.WriteElementString("version", "1");
                if (!String.IsNullOrEmpty(generator))
                    xw.WriteElementString("generator", generator);
                if (!String.IsNullOrEmpty(comment))
                    xw.WriteElementString("comment", comment);
                // Start writing out the <sites> tag
                xw.WriteStartElement("sites");
                // Now step through each site parameter group and write out a <site>
                // tag to contain its data:
                foreach (SiteParameters site in siteList)
                {
                    xw.WriteStartElement("site");
                    xw.WriteElementString("siteToken", site.Site);
                    xw.WriteElementString("hash",
                        HashEngine.HashEnumStringToDisplayHash(site.Hash));
                    xw.WriteElementString("iterations", site.Iterations.ToString());
                    xw.WriteElementString("charTypes", site.CharTypes.ToString());
                    if (site.CharLimit < 0) xw.WriteElementString("charLimit", "0");
                    else xw.WriteElementString("charLimit", site.CharLimit.ToString());
                    xw.WriteEndElement();
                }
                // Close the <sites> tag:
                xw.WriteEndElement();
                // Close the <cryptnos> tag and the rest of the document:
                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Flush();
                xw.Close();
                ms.Flush();
                ms.Close();
                // Get the contents of the memory stream as raw bytes:
                byte[] plaintext = ms.ToArray();
                // Create the cipher.  Note that we're using the encryption
                // mode, and that we're passing in the password:
                BufferedBlockCipher cipher = CreateCipher(password, true);
                // Create our ciphertext container.  Note that we call the
                // cipher's getOutputSize() method, which tells us how big
                // the resulting ciphertext should be.  In practice, this
                // has always been the same size as the plaintext, but we
                // can't take that for granted.
                byte[] ciphertext = new byte[cipher.GetOutputSize(plaintext.Length)];
                // Do the encyrption.  Note that the .NET version is different from
                // the Java version.  Here we've got it easy.  The BC classes include
                // a simpler one-call DoFinal() method that does everything for us.
                ciphertext = cipher.DoFinal(plaintext);
                // Write the ciphertext to the export file:
                FileStream fs = new FileStream(filename, FileMode.Create);
                fs.Write(ciphertext, 0, ciphertext.Length);
                // Close up shop:
                fs.Flush();
                fs.Close();
                plaintext = null;
                ciphertext = null;
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Export a list of <see cref="SiteParameters"/> to an encrypted export file.  Note
        /// that this method only exports to the newer XML-based cross-platform format, not
        /// the old platform specific format that is no longer supported.  By default, the
        /// optional comment field will be omitted.
        /// </summary>
        /// <param name="filename">A string containing the full path to the export file</param>
        /// <param name="password">A string containing the password used to encrypt the
        /// file</param>
        /// <param name="generator">A string containing the "generator" ID for the file, usually
        /// the full application name ("Cryptnos for Windows") and version number.  If this
        /// value is null the generator tag will be omitted from the file.</param>
        /// <param name="siteList">A <see cref="List"/> of <see cref="SiteParameters"/> to
        /// export</param>
        /// <exception cref="ArgumentException">Thrown if any required field (file name,
        /// password, or site list) is empty or null</exception>
        /// <exception cref="Exception">Thrown if anything blows up along the way</exception>
        public static void ExportToFile(string filename, string password, string generator,
            List<SiteParameters> siteList)
        {
            try { ExportToFile(filename, password, generator, null, siteList); }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Export a list of <see cref="SiteParameters"/> to an encrypted export file.  Note
        /// that this method only exports to the newer XML-based cross-platform format, not
        /// the old platform specific format that is no longer supported.  By default, the
        /// optional generator and comment fields will be omitted.
        /// </summary>
        /// <param name="filename">A string containing the full path to the export file</param>
        /// <param name="password">A string containing the password used to encrypt the
        /// file</param>
        /// <param name="siteList">A <see cref="List"/> of <see cref="SiteParameters"/> to
        /// export</param>
        /// <exception cref="ArgumentException">Thrown if any required field (file name,
        /// password, or site list) is empty or null</exception>
        /// <exception cref="Exception">Thrown if anything blows up along the way</exception>
        public static void ExportToFile(string filename, string password,
            List<SiteParameters> siteList)
        {
            try { ExportToFile(filename, password, null, null, siteList); }
            catch (Exception e) { throw e; }
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Given the user's password, generate a salt which will be mixed with the password
        /// when setting up the encryption parameters
        /// </summary>
        /// <param name="password">A string containing the user's password</param>
        /// <returns>An array of bytes containing the raw salt value</returns>
        private static byte[] GenerateSaltFromPassword(string password)
        {
            // Get the password as Unicode (UTF-8) bytes:
            byte[] salt = Encoding.UTF8.GetBytes(password);
            // Try to hash password multiple times using a really strong hash.
            // This should give us some really random-ish data for the salt.
            SHA512Managed hasher = new SHA512Managed();
            for (int i = 0; i < SALT_ITERATION_COUNT; i++)
            {
                // .NET notes:  This isn't quite as simple as the Java version.  Each
                // time we use the hash engine, it must be initialized.  We then have
                // to tell it to transform the final block, which effectively gets it
                // to hash the whole thing.  Then we have to update our salt reference
                // to the new value.
                hasher.Initialize();
                hasher.TransformFinalBlock(salt, 0, salt.Length);
                salt = hasher.Hash;
            }
            return salt;
        }

        /// <summary>
        /// Create the cipher to handle encryption and decryption
        /// </summary>
        /// <param name="password">A string containing the password, which will be used
        /// to derive all our encryption parameters</param>
        /// <param name="encrypt">A boolean value specifying whether we should go into
        /// encryption mode (true) or decryption mode (false)</param>
        /// <returns>A BufferedBlockCipher in the specified mode</returns>
        /// <exception cref="Exception">Thrown whenever anything bad happens</exception>
        private static BufferedBlockCipher CreateCipher(string password, bool encrypt)
        {
            // I tried a dozen different things, none of which seemed to work
            // all that well.  I finally resorted to doing everyting the Bouncy
            // Castle way, simply because it brought things a lot closer to being
            // consistent.  Trying to do things entirely within .NET or Java just
            // wasn't cutting it.  There are, however, differences between the
            // implementations, which are denoted below.
            try
            {
                // Get the password's raw UTF-8 bytes:
                byte[] pwd = Encoding.UTF8.GetBytes(password);
                byte[] salt = GenerateSaltFromPassword(password);
                // From the BC JavaDoc: "Generator for PBE derived keys and IVs as
                // defined by PKCS 5 V2.0 Scheme 2. This generator uses a SHA-1
                // HMac as the calculation function."  This is apparently a standard,
                // which makes my old .NET SecureFile class seem a bit embarrassing.
                Pkcs5S2ParametersGenerator generator = new Pkcs5S2ParametersGenerator();
                // Initialize the generator with our password and salt.  Note the
                // iteration count value.  Examples I found around the net set this
                // as a hex value, but I'm not sure why advantage there is to that.
                // I changed it to decimal for clarity.  1000 iterations may seem
                // a bit excessive, and I saw some real sluggishness on the Android
                // emulator that could be caused by this.  In the final program,
                // this should probably be set in a global app constant.
                generator.Init(pwd, salt, KEY_ITERATION_COUNT);
                // Generate our parameters.  We want to do AES-256, so we'll set
                // that as our key size.  That also implies a 128-bit IV.  Note
                // that the 2-int method used here is considered deprecated in the
                // .NET library, which could be a problem in the long term.  This
                // is where .NET and Java diverge in BC; this is the only method
                // available in Java, and the comparable method is deprecated in
                // .NET.  I'm not sure how this will work going forward.  We need
                // to watch this, as this could be a failure point down the road.
                ParametersWithIV iv =
                    ((ParametersWithIV)generator.GenerateDerivedParameters(KEY_SIZE, IV_SIZE));
                // Create our AES (i.e. Rijndael) engine and create the actual
                // cipher object from it.  We'll use CBC padding.
                RijndaelEngine engine = new RijndaelEngine();
                BufferedBlockCipher cipher =
                    new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
                // Pick our mode, encryption or decryption:
                cipher.Init(encrypt, iv);
                // Return the cipher:
                return cipher;
            }
            // Don't handle exploding things here; pass the buck to the caller:
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// Read a <see cref="List"/> of <see cref="SiteParameters"/> from an original platform
        /// specific Cryptnos for .NET export file
        /// </summary>
        /// <param name="filename">A string containing the full path to the import file</param>
        /// <param name="password">A string containing the password used to decrypt the
        /// file</param>
        /// <returns>A <see cref="List"/> of <see cref="SiteParameters"/></returns>
        /// <exception cref="Exception">Thrown if anything blows up along the way</exception>
        private static List<SiteParameters> ImportFromOriginalExportFile(string filename,
            string password)
        {
            try
            {
                // Read all the raw data of the import file.  Note that SecureFile files are
                // always encrypted.
                byte[] rawData = SecureFile.ReadAllBytes(filename, password);
                // The raw data should contain a binary formatted generic List
                // of SiteParameter objects.  So attempt to deserialize that
                // List:
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(rawData);
                List<SiteParameters> siteParamList = (List<SiteParameters>)bf.Deserialize(ms);
                ms.Close();
                return siteParamList;
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Read a <see cref="List"/> of <see cref="SiteParameters"/> from the new XML-based
        /// cross-platform export file format
        /// </summary>
        /// <param name="filename">A string containing the full path to the import file</param>
        /// <param name="password">A string containing the password used to decrypt the
        /// file</param>
        /// <returns>A <see cref="List"/> of <see cref="SiteParameters"/></returns>
        /// <exception cref="ImportHandlerException">Thrown if a parsing error occurs during
        /// the import process</exception>
        /// <exception cref="Exception">Thrown if anything else blows up along the way</exception>
        private static List<SiteParameters> ImportFromXMLv1File(string filename,
            string password)
        {
            try
            {
                // Try to open a file stream for the file:
                FileStream fs = new FileStream(filename, FileMode.Open);
                // The process below will blow up if we try to read a file that is so large
                // it exceeds the 32-bit integer max value.  This should never happen with an
                // actual Cryptnos export file, but if the user accidentally tries to import
                // a DVD ISO or something, we don't want to blow up their machine.  Bomb out
                // if we find out that the file is too large.
                if (fs.Length > (long)Int32.MaxValue)
                {
                    fs.Close();
                    throw new ImportHandlerException("Import file too large to load into memory");
                }
                // Read the entire contents of the file into memory:
                byte[] contents = new byte[(int)fs.Length];
                fs.Read(contents, 0, contents.Length);
                fs.Close();
                // Create our cipher in decryption mode:
                BufferedBlockCipher cipher = CreateCipher(password, false);
                // Create our plaintext container:
                byte[] plaintext = new byte[cipher.GetOutputSize(contents.Length)];
                // Decrypt the data and create a memory stream so we can read from it:
                plaintext = cipher.DoFinal(contents);
                MemoryStream ms = new MemoryStream(plaintext);
                contents = null;
                // Define our XML reader settings:
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                // Note that we'll point to our local copy of the XSD, which should be in the
                // application directory:
                xmlReaderSettings.Schemas.Add("http://www.cryptnos.com/",
                    Application.StartupPath + Char.ToString(System.IO.Path.DirectorySeparatorChar) +
                    "cryptnos_export1.xsd");
                // Validate against the schema.  Invalid files will throw exceptions:
                xmlReaderSettings.ValidationType = ValidationType.Schema;
                xmlReaderSettings.ValidationEventHandler +=
                    new ValidationEventHandler(xmlReaderSettings_ValidationEventHandler);
                // Ignore unnecessary information:
                xmlReaderSettings.IgnoreComments = true;
                xmlReaderSettings.IgnoreWhitespace = true;
                // Close any other file/input streams when we close this one:
                xmlReaderSettings.CloseInput = true;
                // Create the XML reader.  Note that we're reading from the memory stream
                // created from the decrypted file, then passing that through a gzip
                // decompressor before actually getting to the data.
                XmlReader xr = XmlReader.Create(new GZipStream(ms,
                    CompressionMode.Decompress), xmlReaderSettings);
                // This forces us to go to the first element, which should be <cryptnos>.  If
                // not, complain:
                xr.MoveToContent();
                if (xr.Name != "cryptnos")
                    throw new ImportHandlerException("Invalid Cryptnos export file; expected <cryptnos> tag but got <" + xr.Name + ">");
                // At this point, things are looking good.  We'll hopefully have sites we can
                // import now.  Go ahead and create our List of SiteParameters so we can start
                // building it:
                List<SiteParameters> siteList = new List<SiteParameters>();
                // Read the next element.  This should be a <version> tag.  If it is, make sure
                // it's a version we recognize.  Otherwise, complain.
                xr.Read();
                if (xr.NodeType != XmlNodeType.Element)
                    throw new ImportHandlerException("Invalid Cryptnos export file; expected an element, but got " + xr.NodeType.ToString());
                if (xr.Name.CompareTo("version") == 0)
                {
                    xr.Read();
                    // Make sure this is a text value, then make sure it's the version
                    // number we expect.  This version of Cryptnos only accepts version
                    // 1 of the Cryptnos export file format.
                    if (xr.NodeType == XmlNodeType.Text && xr.Value != "1")
                        throw new ImportHandlerException("This Cryptnos export file appears to have been generated by a later version of Cryptnos and is incompatible with this version. (File format version was " + xr.Value + ".)");
                }
                else throw new ImportHandlerException("Invalid Cryptnos export file; expected a <version> element, but got <" + xr.Name + ">");
                // Read on to the next tag:
                xr.Read();
                while (xr.NodeType == XmlNodeType.EndElement) xr.Read();
                if (xr.NodeType != XmlNodeType.Element)
                    throw new ImportHandlerException("Invalid Cryptnos export file; expected an element, but got " + xr.NodeType.ToString());
                // At this point, the next few tags should be the <generator> and/or <comment>
                // tags, neither of which we care about.  Therefore, just read ahead until we
                // hit the <sites> tag, which is where we really want to go to next.
                while (xr.Name.CompareTo("sites") != 0)
                {
                    do xr.Read(); while (xr.NodeType != XmlNodeType.Element);
                }
                // Now we need to check to make sure we actually got a <sites> tag:
                if (xr.NodeType == XmlNodeType.Element && xr.Name.CompareTo("sites") == 0)
                {
                    // Read the next tag.  This should be a <site> tag and the beginning of a
                    // site defintion.
                    xr.Read();
                    while (xr.NodeType == XmlNodeType.Element && xr.Name.CompareTo("site") == 0)
                    {
                        // Create a new SiteParameters object to store the data we're about
                        // to read.
                        SiteParameters site = new SiteParameters();
                        // Try to read the <siteToken> tag:
                        xr.Read();
                        if (xr.NodeType != XmlNodeType.Element)
                            throw new ImportHandlerException("Invalid Cryptnos export file; expected an element, but got " + xr.NodeType.ToString());
                        if (xr.Name.CompareTo("siteToken") == 0)
                        {
                            xr.Read();
                            if (xr.NodeType == XmlNodeType.Text) site.Site = xr.Value;
                            else throw new ImportHandlerException("Invalid site token (" + xr.Value + ")");
                        }
                        else throw new ImportHandlerException("Invalid Cryptnos export file; expected a <siteToken> element, but got <" + xr.Name + ">");
                        do xr.Read(); while (xr.NodeType != XmlNodeType.Element);
                        // Try to read the <hash> tag:
                        if (xr.NodeType != XmlNodeType.Element)
                            throw new ImportHandlerException("Invalid Cryptnos export file; expected an element, but got " + xr.NodeType.ToString());
                        if (xr.Name.CompareTo("hash") == 0)
                        {
                            xr.Read();
                            if (xr.NodeType == XmlNodeType.Text)
                                site.Hash = HashEngine.DisplayHashToHashEnumString(xr.Value);
                            else throw new ImportHandlerException("Invalid hash token (" + xr.Value + ")");
                        }
                        else throw new ImportHandlerException("Invalid Cryptnos export file; expected a <hash> element, but got <" + xr.Name + ">");
                        do xr.Read(); while (xr.NodeType != XmlNodeType.Element);
                        // Try to read the <iterations> tag:
                        if (xr.NodeType != XmlNodeType.Element)
                            throw new ImportHandlerException("Invalid Cryptnos export file; expected an element, but got " + xr.NodeType.ToString());
                        if (xr.Name.CompareTo("iterations") == 0)
                        {
                            xr.Read();
                            if (xr.NodeType == XmlNodeType.Text) site.Iterations = Int32.Parse(xr.Value);
                            else throw new ImportHandlerException("Invalid iterations token (" + xr.Value + ")");
                        }
                        else throw new ImportHandlerException("Invalid Cryptnos export file; expected an <iterations> element, but got <" + xr.Name + ">");
                        do xr.Read(); while (xr.NodeType != XmlNodeType.Element);
                        // Try to read the <charTypes> tag:
                        if (xr.NodeType != XmlNodeType.Element)
                            throw new ImportHandlerException("Invalid Cryptnos export file; expected an element, but got " + xr.NodeType.ToString());
                        if (xr.Name.CompareTo("charTypes") == 0)
                        {
                            xr.Read();
                            if (xr.NodeType == XmlNodeType.Text) site.CharTypes = Int32.Parse(xr.Value);
                            else throw new ImportHandlerException("Invalid charTypes token (" + xr.Value + ")");
                        }
                        else throw new ImportHandlerException("Invalid Cryptnos export file; expected a <charTypes> element, but got <" + xr.Name + ">");
                        do xr.Read(); while (xr.NodeType != XmlNodeType.Element);
                        // Try to read the <charLimit> tag:
                        if (xr.NodeType != XmlNodeType.Element)
                            throw new ImportHandlerException("Invalid Cryptnos export file; expected an element, but got " + xr.NodeType.ToString());
                        if (xr.Name.CompareTo("charLimit") == 0)
                        {
                            xr.Read();
                            if (xr.NodeType == XmlNodeType.Text)
                            {
                                // The character limit, unfortunately, is a bit inconsistent.
                                // Obviously, we can't limit it to zero, as that will mean we
                                // have an empty password.  But the XML schema defines this as
                                // only positive integers, so our built-in -1 value can't work.
                                // So if we read a zero, convert it to -1 here.  We'll do the
                                // reverse when we write the file.
                                site.CharLimit = Int32.Parse(xr.Value);
                                if (site.CharLimit == 0) site.CharLimit = -1;
                            }
                            else throw new ImportHandlerException("Invalid charLimit token (" + xr.Value + ")");
                        }
                        else throw new ImportHandlerException("Invalid Cryptnos export file; expected a <charLimit> element, but got <" + xr.Name + ">");
                        // The next item should be the closing element for <charLimit>, so
                        // read it in and then read the next one.  That should either be the
                        // start element for the next <site> or the closing element for
                        // <sites>.
                        xr.Read();
                        if (xr.NodeType == XmlNodeType.EndElement) xr.Read();
                        else throw new ImportHandlerException("Invalid Cryptnos export file; expected ending <charLimit> tag, got " + xr.NodeType.ToString());
                        if (xr.NodeType == XmlNodeType.EndElement) xr.Read();
                        else throw new ImportHandlerException("Invalid Cryptnos export file; expected ending <site> tag, got " + xr.NodeType.ToString());
                        // We should now have a hopefully valid SiteParameters object.  Add it
                        // to the site list:
                        siteList.Add(site);
                    }
                    // We get here, we've exhausted the <sites> block and all that should be
                    // left will be closing tags.  If we were going to be extremely thorough,
                    // we should probably check these closing tags and make sure they're legit.
                    // For now, we'll just assume there's nothing left to read.  Close the
                    // streams, free up memory, and return the list of read sites.
                    xr.Close();
                    ms.Close();
                    ms.Dispose();
                    plaintext = null;
                    return siteList;
                }
                else throw new ImportHandlerException("Invalid Cryptnos export file; could not find <sites> tag");
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// This event handler handles XML validation problems.  Right now, all it does is print
        /// out a message box indicating that the validation problem occurred.  Ideally, we need
        /// to find a better way to handle this, which *could* mean not handling these events
        /// at all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void xmlReaderSettings_ValidationEventHandler(object sender,
            ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                MessageBox.Show("XML Validation Warning: " + e.Message,
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                MessageBox.Show("XML Validation Error: " + e.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        #endregion

    }
}
