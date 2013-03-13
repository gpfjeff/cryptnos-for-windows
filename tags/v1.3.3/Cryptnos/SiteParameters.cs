/* SiteParameters.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 21, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      Form1
 * REQUIRED BY:   (None)
 * 
 * The SiteParameters object encapsulates an atomic set of Cryptnos parameters for generating
 * a secure passphrase for a given site.  While the initial design of Cryptnos did not include
 * a separate data class for these parameters, as more and more features were added it became
 * apparent that a more object oriented approach was necessary.  Specifically, the import/export
 * feature of Cryptnos lent itself well to pursuing serialization, and this led to the idea of
 * encrypting site parameters before storing them in the registry.
 * 
 * Note that in every case, one key element is *NEVER* stored:  the user's secret passphrase.
 * If you consider a two-factor authentication system, there is something that you have and
 * something that you know.  Only when the two items are combined will the system be unlocked.
 * Typically, the "something that you know" item is a secret passphrase that the user memorizes,
 * while the "something that you have" item is a physical object like a smart card or
 * random(-ish) number token.  In terms of client-side certificates, the certificate becomes
 * the "have" object and its passphrase the "know".  In this case, the SiteParamemters object
 * becomes the "have", storing all the information needed to generate the Cryptnos passphrase
 * *EXCEPT* for the user's secret passphrase itself.  That becomes the "know" portion, and only
 * when the two are combined can the Cryptnos passphrase be generated.
 * 
 * This class is serializable, meaning it can be serialized for import and export.  This has
 * two consequences.  For the Cryptnos import/export feature, this allows the parameters to
 * be serialized so they can be exported to a file, then deserialized for import somewhere
 * else.  Note that in this case the serialization routine only bothers with serialization
 * and *NOT* encryption; Cryptnos itself performs encryption/decryption on the export file,
 * which is keyed to a passphrase entered by the user.
 * 
 * The other feature of serialization is the encryption of data stored in the Windows registry.
 * Rather than storing the site parameters in plain text (the original plan for the program),
 * we now serialize the parameters and encrypt them, then save the encrypted data to the
 * registry.  In this case, the SiteParmemeters methods does the actual encryption/decryption,
 * so Cryptnos only need to call the appropriate methods.
 * 
 * UPDATES FOR 1.3.3:  Added comments on why Encoding.Default is used in certain places rather
 * than switching those to Encoding.UTF8 as we've done other places.
 * 
 * This program is Copyright 2012, Jeffrey T. Darlington.
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
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// This class serves as a mechanism for serializing and exporting Cryptnos site parameter
    /// data.
    /// </summary>
    [Serializable]
    public class SiteParameters : ISerializable
    {
        #region Private Variables

        /// <summary>
        /// The full site name string
        /// </summary>
        private string site = null;

        /// <summary>
        /// The index of the character types drop-down list
        /// </summary>
        private int charTypes = -1;

        /// <summary>
        /// A limit on the number of characters to return.  Must be -1 or greater than zero.
        /// If -1, it is implied that there will be no character limit enforced.
        /// </summary>
        private int charLimit = -1;

        /// <summary>
        /// A string identifying the hash used to generate the password
        /// </summary>
        private string hash = null;

        /// <summary>
        /// A positive integer greater than zero representing the number of times the
        /// specified hash algorithm should be applied to the inputs.
        /// </summary>
        private int iterations = 1;

        /// <summary>
        /// A generated registry key value used to look up this set of parameters
        /// </summary>
        private string key = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// The full site name string
        /// </summary>
        public string Site
        {
            get { return site; }
            set
            {
                site = value;
                key = GenerateKeyFromSite(site);
            }
        }

        /// <summary>
        /// The index of the character types drop-down list
        /// </summary>
        public int CharTypes
        {
            get { return charTypes; }
            set { charTypes = value; }
        }

        /// <summary>
        /// A limit on the number of characters to return.  Must be -1 or greater than zero.
        /// If -1, it is implied that there will be no character limit enforced.
        /// </summary>
        public int CharLimit
        {
            get { return charLimit; }
            set { charLimit = value; }
        }

        /// <summary>
        /// A string identifying the hash used to generate the password
        /// </summary>
        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        /// <summary>
        /// A positive integer greater than zero representing the number of times the
        /// specified hash algorithm should be applied to the inputs.
        /// </summary>
        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; }
        }

        /// <summary>
        /// A read-only property of the generated registry subkey name for storing the site
        /// parameters in the registry.  This will only be available if the <see cref="Site"/>
        /// property is populated; otherwise it will be null.
        /// </summary>
        public string Key
        {
            get
            {
                // Since generating the key involves some expensive hashing, only
                // regenerate it if it hasn't been generated already:
                if (!String.IsNullOrEmpty(key)) return key;
                // Otherwise generate the key and return it:
                else
                {
                    key = GenerateKeyFromSite(site);
                    return key;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor.  Note that the site parameters will be completely
        /// useless unless manually populated.
        /// </summary>
        public SiteParameters()
        {
        }

        /// <summary>
        /// A constructor taking all the basic parameters
        /// </summary>
        /// <param name="site">The full site name</param>
        /// <param name="charTypes">An integer index of the character type drop-down</param>
        /// <param name="charLimit">The character limit, -1 for none</param>
        /// <param name="hash">A string identifying the hash used</param>
        /// <param name="iterations">A positive integer greater than zero representing the
        /// number of iterations of the hash to perform</param>
        public SiteParameters(string site, int charTypes, int charLimit, string hash,
            int iterations)
        {
            this.site = site;
            this.charTypes = charTypes;
            this.charLimit = charLimit;
            this.hash = hash;
            this.iterations = iterations;
            key = GenerateKeyFromSite(site);
        }

        /// <summary>
        /// A constructor for deserializing SiteParameter objects.  If deserialization
        /// fails, a useless dummy object will be returned.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SiteParameters(SerializationInfo info, StreamingContext context)
        {
            try
            {
                site = info.GetString("Site");
                charTypes = info.GetInt32("CharTypes");
                charLimit = info.GetInt32("CharLimit");
                hash = info.GetString("Hash");
                iterations = info.GetInt32("Iterations");
                key = GenerateKeyFromSite(site);
            }
            catch
            {
                site = null;
                charTypes = -1;
                charLimit = -1;
                hash = null;
                iterations = 1;
                key = null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given an open registry key, save this set of site parameters as a subkey under
        /// that registry, encrypting the data as we go.
        /// </summary>
        /// <param name="registryKey">The parent registry key</param>
        /// <returns>True for success, false for failure</returns>
        public bool SaveToRegistry(RegistryKey registryKey)
        {
            // This only makes sense if the registry key exists, i.e. it is open:
            if (registryKey != null)
            {
                // Asbestos underpants:
                try
                {
                    // Serialize the parameters binary data:
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, this);
                    ms.Close();
                    byte[] serializedParams = ms.ToArray();
                    // Generate the Rijndael object, encryption key and initialization vector:
                    RijndaelManaged rijndael = new RijndaelManaged();
                    if (rijndael.ValidKeySize(256)) rijndael.KeySize = 256;
                    rijndael.Padding = PaddingMode.PKCS7;
                    byte[] cryptKey = GenerateEncryptionKey(Key);
                    byte[] iv = GenerateIV(rijndael.BlockSize / 8, Key);
                    // Encrypt the site parameters:
                    ICryptoTransform encryptor = rijndael.CreateEncryptor(cryptKey, iv);
                    ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                    cs.Write(serializedParams, 0, serializedParams.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                    ms.Close();
                    // Now convert the data to Base64 and save it to the registry using the
                    // generated key
                    registryKey.SetValue(Key, ms.ToArray(), RegistryValueKind.Binary);
                    return true;
                }
                // If anything failed, let the user know:
                catch { return false; }
            }
            // If the registry key wasn't open, there's no use continuing:
            else return false;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Given an open registry key and a site key value, read the site parameters from
        /// the registry and return a SiteParameters object.
        /// </summary>
        /// <param name="registryKey">An open registry key</param>
        /// <param name="siteKey">A site key string</param>
        /// <returns>A SiteParameters object, or null on failure</returns>
        public static SiteParameters ReadFromRegistry(RegistryKey registryKey, string siteKey)
        {
            // Asbestos underpants:
            try
            {
                // This only works if the registry key is open and the site key is
                // something meaningful:
                if (registryKey != null && !String.IsNullOrEmpty(siteKey))
                {
                    // Look for the site key value in the registry key and convert it from
                    // Base64 to a byte array:
                    byte[] encryptedParams = (byte[])registryKey.GetValue(siteKey);
                    // Set up our decryption engine.  Create the Rijndael object, its
                    // encryption key, and its initialization vector.
                    RijndaelManaged rijndael = new RijndaelManaged();
                    if (rijndael.ValidKeySize(256)) rijndael.KeySize = 256;
                    rijndael.Padding = PaddingMode.PKCS7;
                    byte[] cryptKey = GenerateEncryptionKey(siteKey);
                    byte[] iv = GenerateIV(rijndael.BlockSize / 8, siteKey);
                    // Decrypt the raw bytes read from the registry:
                    ICryptoTransform decryptor = rijndael.CreateDecryptor(cryptKey, iv);
                    MemoryStream ms = new MemoryStream(encryptedParams);
                    CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                    byte[] decryptedBytes = new byte[encryptedParams.Length];
                    cs.Read(decryptedBytes, 0, decryptedBytes.Length);
                    cs.Close();
                    ms.Close();
                    // Reset the memory stream to read the raw bytes and use a binary
                    // formatter to deserialize the object:
                    ms = new MemoryStream(decryptedBytes);
                    BinaryFormatter bf = new BinaryFormatter();
                    SiteParameters sp = (SiteParameters)bf.Deserialize(ms);
                    return sp;
                }
                // If the registry key wasn't open or the site key wasn't meaningful, there's
                // nothing to do:
                else return null;
            }
            // If anything blows up, don't return anything we can use:
            catch { return null; }
        }

        /// <summary>
        /// Given a string for a site name, generate the registry subkey name
        /// </summary>
        /// <param name="site">The site name to get the key from</param>
        /// <returns>The generated registry key string, or null on failure</returns>
        public static string GenerateKeyFromSite(string site)
        {
            try
            {
                // Otherwise, only bother if we have a site to work with:
                if (!String.IsNullOrEmpty(site) && site.Length > 0)
                {
                    // Get the SHA-512 of the site name.  We'll also include the user's
                    // username and the machine name as a salt.  We really should change
                    // the encoding here to something like UTF-8, but we'll leave this
                    // at the default to keep from breaking the user's data.
                    SHA512Managed hasher = new SHA512Managed();
                    byte[] keyBytes = Encoding.Default.GetBytes(site + Environment.UserName +
                        Environment.MachineName);
                    keyBytes = hasher.TransformFinalBlock(keyBytes, 0, keyBytes.Length);
                    // Convert the binary hash to Base64, then chop it down to 255 characters
                    // if it's too long.  Windows registry keys can only be 255 characters
                    // in length.
                    string key = Convert.ToBase64String(keyBytes);
                    if (key.Length > 255) return key.Substring(0, 255);
                    else return key;
                }
                else return null;
            }
            catch { return null; }
        }

        #endregion

        #region ISerializable Members

        /// <summary>
        /// Serialize the SiteParameters object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Site", site);
            info.AddValue("CharTypes", charTypes);
            info.AddValue("CharLimit", charLimit);
            info.AddValue("Hash", hash);
            info.AddValue("Iterations", iterations);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate an encryption key for encrypting site parameters.
        /// </summary>
        /// <returns>A byte array containing the encryption key.</returns>
        private static byte[] GenerateEncryptionKey(string key)
        {
            // Generate an encryption key to encrypt with.  This is based on the
            // machine name and username of the user, which probably aren't the best
            // choices, but should be unique for each user/machine.  Note that we're
            // using the system default encoding here, rather than UTF-8; this is a bit
            // of legacy code, and we can't really change this without breaking existing
            // parameters.  Since this is an internal thing, so it shouldn't be a problem.
            SHA256Managed hasher = new SHA256Managed();
            byte[] cryptKey = Encoding.Default.GetBytes(Environment.MachineName +
                Environment.UserName + key);
            hasher.TransformFinalBlock(cryptKey, 0, cryptKey.Length);
            return hasher.Hash;
        }

        /// <summary>
        /// Given a size in bytes, generate an initialization vector (IV) for encryption
        /// </summary>
        /// <param name="size">Size of the output byte array</param>
        /// <returns>An array of bytes to be used as the IV</returns>
        private static byte[] GenerateIV(int size, string key)
        {
            // Use SHA-512 to generate a pseudo-random IV.  We'll use the user's username and
            // the machine name as the key data; this isn't really super secure, but it should
            // at least be unique per user/machine.  Note that we're using the system default
            // encoding here, rather than UTF-8; this is a bit of legacy code, and we can't
            // really change this without breaking existing parameters.  Since this is an
            // internal thing, so it shouldn't be a problem.
            SHA512Managed hasher = new SHA512Managed();
            byte[] iv = Encoding.Default.GetBytes(key + Environment.UserName +
                Environment.MachineName);
            hasher.TransformFinalBlock(iv, 0, iv.Length);
            iv = hasher.Hash;
            // SHA-512 is probably overkill for the IV we need.  So given the size value,
            // chop of only the number of bytes we need and return those.
            if (iv.Length > size)
            {
                byte[] slice = new byte[size];
                Array.Copy(iv, 0, slice, 0, size);
                return slice;
            }
            else return iv;
        }

        #endregion

    }
}
