/* SecureFile.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 17, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      SecureFileException.cs
 * REQUIRED BY:   (None)
 * 
 * SecureFile is a very simple static utility class for reading and writing encrypted file data.
 * It uses strong 256-bit Rijndael encryption and compresses the data to minimize file sizes.  It
 * is intended to be extremely easy to use, making it virtually trivial for anyone to create data
 * files that are strongly encrypted.
 * 
 * SecureFile is actually a hold-over from another project I worked on, but was used in Cryptnos
 * for the encryption of the original platform-specific export file format.  Newer versions of
 * Cryptnos use "proper" AES-256 encryption scheme rather than this custom class, so this code
 * only remains within Cryptnos to support reading the old original export format.  Its use for
 * virtually any other purpose should probably be discouraged.
 * 
 * The static Write() methods always take a minimum of three arguments:  a string containing the
 * file name (and path) to work with, some sort of data to write to that file, and a string containing
 * the passphrase to encrypt the data with.  An optional initialization vector (IV) can be specified,
 * either as an array of raw bytes or as a Base64-encoded string.  If not specified, an IV will be
 * derived from the supplied passphrase.  The data may either be a raw array of bytes or a string,
 * which defaults to UTF-8 but you can override the encoding to whatever you wish.  If a file by the
 * specified name already exists, it will be overwritten.  All Write() methods return void.
 * 
 * The static Read...() methods take at least two arguments:  the file path string and the pass-
 * phrase string.  If an IV is not supplied (either as a byte array or Base64-encoded string), it
 * will be derived using the same method as used bye the Write()s.  ReadAllBytes() will read the
 * file's contents as raw bytes and return a byte array.  The ReadAsString() methods will read the
 * file's contents as a string, by default with UTF-8 encoding or with an encoding you specify.
 * 
 * All public methods may potentially throw a SecureFileException.  No other exceptions should be
 * thrown; even generic exceptions should be wrapped in SecureFileExceptions.  Check the Message
 * property to see why the exception was thrown.  These are usually simple enough that they can be
 * easily added to message boxes or printed out on the command line.
 * 
 * A note about file sizes:  SecureFile is not intended for very large files.  In fact, it actually
 * has a hard limit of 2GB (Int32.MaxValue), but anything of significant size should probably be
 * avoided.  There's a lot of overhead in compressing and encrypting this data, and the same data
 * may end up in memory several times in various states before anything gets recorded to disk or
 * returned from a read.  Thus, smaller files will likely work better than larger ones.
 * 
 * A note about the encryption used in SecureFile:  Those with in-depth knowledge of cryptography
 * will be aware that Rijndael encryption requires both an encryption key (the passphrase) and an
 * initialization vector (IV), and that both are required to decrypt something encrypted with this
 * pair of inputs.  I intentionally wanted to create a system that used a single passphrase for
 * encryption so that the file as portable as possible.  While some methods allow you to specify an
 * IV, other methods generate a default IV that is derived from the passphrase using a series of
 * cryptographic hashes and string manipulation.  In theory, this might be conceived as a weakness,
 * since only the single passphrase is required to decrypt the data.  However, I don't believe it
 * would be easy to reverse engineer the IV from the encrypted data, and that the derived IV will be
 * "strong enough" for a crypto system using a single passphrase.  If this is a serious concern,
 * either (a) specify a unique IV for each encrypted file and keep a reference for that IV for later
 * use, (b) use a different passphrase for every encrypted SecureFile, or (c) don't use SecureFile
 * at all.
 * 
 * Of course, *serious* crypto-heads will likely take exception at doing *anything* seriously
 * security conscious on Windows at all.  Note that while encryption is taking place, memory on
 * Windows machines is not very well protected and may be read by malicious processes running on
 * the same machine.  Similarly, the contents of memory could be swapped out to the paging file
 * (virtual memory) at any time, meaning unencrypted data may end up on your hard disk and recoverable
 * by someone proficient enough to find it.  It is assumed that if you're using this library that you
 * are aware of these risks and that you deem the data you're protecting to not be sensitive enough
 * to be disuaded by these risks.  Once the information is encrypted and written to disk, it should
 * be relatively safe; but anytime before encryption or after decryption it could be vulnerable.
 * Please practice safe and secure computing practices regardless of your operating system choice.
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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// A simple static utility class for reading and writing encrypted file data.
    /// </summary>
    public static class SecureFile
    {
        #region Private Variables

        /// <summary>
        /// A managed Rijndael (AES) cryptographic engine, which will do the grunt work of
        /// encrypting our data
        /// </summary>
        private static RijndaelManaged rijndael = new RijndaelManaged();

        /// <summary>
        /// We will assume all data coming in and out will be UTF-8 encoded, so this reference
        /// will let us access that information consistently
        /// </summary>
        private static UTF8Encoding utf8 = new UTF8Encoding();

        /// <summary>
        /// This string will be combined with the passphrase to give us plenty of entropy and
        /// make the encryption key stronger than just using the passphrase alone.  If you
        /// don't like Moby Dick, feel free to change this, but keep in mind that anything
        /// encrypted before this is changed can't be decrypted!
        /// </summary>
        private static string salt =
            "Towards thee I roll, thou all-destroying but unconquering whale; to the last " +
            "I grapple with thee; from hell's heart I stab at thee; for hate's sake I " +
            "spit my last breath at thee.  Sink all coffins and all hearses to one common " +
            "pool!  and since neither can be mine, let me then tow to pieces, while still " +
            "chasing thee, though tied to thee, thou damned whale!  Thus, I give up the " +
            "spear!";

        /// <summary>
        /// A header string to let us know whether or not the decryption was successful
        /// </summary>
        private static string header = "##SecureFile##";

        /// <summary>
        /// How much padding to fudge with when uncompressing data.  GZipStream doesn't let us know
        /// how big the uncompressed data will be, so we have to fake it.  We'll multiply the
        /// compressed file's size in bytes by this value.  Increase this value if we consistently
        /// run out of space, or decrease it if it's found that we're wasting too much space.
        /// </summary>
        private static int uncompressArrayPadding = 100;

        #endregion

        #region Write Methods

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A byte array containing the data to encrypt</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        /// <param name="iv">A byte array containing the initialization vector (IV) which will
        /// encrypt the data</param>
        public static void Write(string filename, byte[] data, string passphrase, byte[] iv)
        {
            try
            {
                // Create the output file stream.  It may seem a bit odd to do this first, before we
                // have any data to write, but this will ensure we throw any file access exceptions
                // before we get to the expensive compression/encryption stuff.
                FileStream output = new FileStream(filename, FileMode.Create);

                // The first thing we have to do is append the header to the data.  If we don't, we
                // can't guarantee later that the passphrase used to decrypt the data is the same as
                // the passphrase used to encrypt it.  So convert the header string to bytes and stick
                // those bytes to the beginning of the data.
                byte[] headerBytes = utf8.GetBytes(header);
                byte[] dataWithHeader = new byte[data.Length + headerBytes.Length];
                Array.Copy(headerBytes, dataWithHeader, headerBytes.Length);
                Array.Copy(data, 0, dataWithHeader, headerBytes.Length, data.Length);

                // First we want to compress the data.  Create a memory stream to write to, then wrap
                // a GZipStream around that.  Write the data bytes to the compressed stream.  Make
                // sure to close the streams before doing anything with the data itself.
                MemoryStream compressedStream = new MemoryStream();
                GZipStream zipper = new GZipStream(compressedStream, CompressionMode.Compress);
                zipper.Write(dataWithHeader, 0, dataWithHeader.Length);
                zipper.Close();
                compressedStream.Close();

                // Now we need to get the compressed data back out and into a byte array we can feed
                // to the encryptor:
                byte[] compressedData = compressedStream.ToArray();

                // Create a new Rijndael instance and set its key size to 256-bits, the highest
                // we can go.  Also set our padding mode to PKCS #7, which will make it easier for
                // us to tell where the data ends and the padding begins when we decrypt this later.
                // Finally, create a crypto transform using that instance and feed it the hashed
                // password and derived IV.
                if (rijndael.ValidKeySize(256)) rijndael.KeySize = 256;
                rijndael.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor =
                    rijndael.CreateEncryptor(HashPassphrase(passphrase), iv);

                // Next, encrypt the data.  We'll reuse the memory stream we had before and create a
                // crypto stream to wrap around it.  Write the compressed data into the encryption
                // stream.  Again, close the streams before reading the data; this is important here
                // so the padding gets done correctly.
                compressedStream = new MemoryStream();
                CryptoStream cs = new CryptoStream(compressedStream, encryptor,
                    CryptoStreamMode.Write);
                cs.Write(compressedData, 0, compressedData.Length);
                cs.FlushFinalBlock();
                cs.Close();
                compressedStream.Close();

                // Get the newly encrypted, compressed data back into byte array form:
                byte[] encryptedData = compressedStream.ToArray();

                // Now that we have the encrypted data, write it to the file and close up shop:
                output.Write(encryptedData, 0, encryptedData.Length);
                output.Flush();
                output.Close();
            }
            #region Catching Exceptions
            catch (UnauthorizedAccessException)
            {
                throw new SecureFileException("You do not have the necessary permissions to access " +
                    "the file " + filename);
            }
            catch (System.Security.SecurityException)
            {
                throw new SecureFileException("You do not have the necessary permissions to access " +
                    "the file " + filename);
            }
            catch (ArgumentException)
            {
                throw new SecureFileException("The specified file path was empty or contained " +
                    "invalid characters");
            }
            catch (IOException)
            {
                throw new SecureFileException("An I/O error occurred while trying to write to " +
                    "the file " + filename);
            }
            // A generic catch-all, just in case:
            catch (Exception ex)
            {
                throw new SecureFileException(ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A string containing the data to encrypt</param>
        /// <param name="encoder">The text encoding scheme (ASCII, UTF-8, etc.) that describes how
        /// the string data is stored.  The same encoding scheme must be used when encrypting and
        /// and unencrypting the data.</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        /// <param name="iv">A byte array containing the initialization vector (IV) which will
        /// encrypt the data</param>
        public static void Write(string filename, string data, Encoding encoder, string passphrase,
            byte[] iv)
        {
            try
            {
                Write(filename, encoder.GetBytes(data), passphrase, iv);
            }
            #region Catch Exceptions
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The specified data string was empty");
            }
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A string containing the data to encrypt.  The string is assumed to
        /// be in UTF-8 format.</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        /// <param name="iv">A byte array containing the initialization vector (IV) which will
        /// encrypt the data</param>
        public static void Write(string filename, string data, string passphrase, byte[] iv)
        {
            try
            {
                Write(filename, utf8.GetBytes(data), passphrase, iv);
            }
            #region Catch Exceptions
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The specified data string was empty");
            }
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A byte array containing the data to encrypt</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        /// <param name="iv">A Base64-encoded string containing the initialization vector (IV)
        /// which will encrypt the data</param>
        public static void Write(string filename, byte[] data, string passphrase, string iv)
        {
            try
            {
                Write(filename, data, passphrase, Convert.FromBase64String(iv));
            }
            #region Catch Exceptions
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The initialization vector string is empty");
            }
            catch (FormatException)
            {
                throw new SecureFileException("The initialization vector string is invalid");
            }
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A string containing the data to encrypt</param>
        /// <param name="encoder">The text encoding scheme (ASCII, UTF-8, etc.) that describes how
        /// the string data is stored.  The same encoding scheme must be used when encrypting and
        /// and unencrypting the data.</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        /// <param name="iv">A Base64-encoded string containing the initialization vector (IV)
        /// which will encrypt the data</param>
        public static void Write(string filename, string data, Encoding encoder, string passphrase,
            string iv)
        {
            try
            {
                Write(filename, encoder.GetBytes(data), passphrase, Convert.FromBase64String(iv));
            }
            #region Catch Exceptions
            catch (ArgumentNullException)
            {
                throw new SecureFileException("One or both of the data or initialization vector " +
                    "strings are empty");
            }
            catch (FormatException)
            {
                throw new SecureFileException("The initialization vector string is invalid");
            }
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A string containing the data to encrypt.  The string is assumed to
        /// be in UTF-8 format.</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        /// <param name="iv">A Base64-encoded string containing the initialization vector (IV)
        /// which will encrypt the data</param>
        public static void Write(string filename, string data, string passphrase, string iv)
        {
            try
            {
                Write(filename, utf8.GetBytes(data), passphrase, Convert.FromBase64String(iv));
            }
            #region Catch Exceptions
            catch (ArgumentNullException)
            {
                throw new SecureFileException("One or both of the data or initialization vector " +
                    "strings are empty");
            }
            catch (FormatException)
            {
                throw new SecureFileException("The initialization vector string is invalid");
            }
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }
        
        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A byte array containing the data to encrypt</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        public static void Write(string filename, byte[] data, string passphrase)
        {
            try
            {
                Write(filename, data, passphrase, GenerateIV(passphrase, rijndael.BlockSize));
            }
            #region Catch Exceptions
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A string containing the data to encrypt</param>
        /// <param name="encoder">The text encoding scheme (ASCII, UTF-8, etc.) that describes how
        /// the string data is stored.  The same encoding scheme must be used when encrypting and
        /// and unencrypting the data.</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        public static void Write(string filename, string data, Encoding encoder, string passphrase)
        {
            try
            {
                Write(filename, encoder.GetBytes(data), passphrase,
                    GenerateIV(passphrase, rijndael.BlockSize));
            }
            #region Catch Exceptions
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The data string is empty");
            }
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }

        /// <summary>
        /// Encrypt the specified data and write it out to the specified file name
        /// </summary>
        /// <param name="filename">A string containing the path to the file to create.  If the file
        /// already exists, it will be overwritten.</param>
        /// <param name="data">A string containing the data to encrypt.  The string is assumed to
        /// be in UTF-8 format.</param>
        /// <param name="passphrase">A string containing the passphrase which will encrypt
        /// the data</param>
        public static void Write(string filename, string data, string passphrase)
        {
            try
            {
                Write(filename, utf8.GetBytes(data), passphrase,
                    GenerateIV(passphrase, rijndael.BlockSize));
            }
            #region Catch Exceptions
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The data string is empty");
            }
            // Rethrow any exceptions we threw in the called method:
            catch (SecureFileException sfe) { throw sfe; }
            #endregion
        }

        #endregion

        #region Read Methods

        /// <summary>
        /// Read all bytes from an encrypted SecureFile
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <param name="iv">A byte array containing the initialization vector (IV) used to encrypt
        /// the orginal file</param>
        /// <returns>An array of bytes representing the unecrypted file's contents</returns>
        public static byte[] ReadAllBytes(string filename, string passphrase, byte[] iv)
        {
            try
            {
                // Only do this if the specified file exists:
                if (File.Exists(filename))
                {
                    // Open the file through a FileStream:
                    FileStream fs = new FileStream(filename, FileMode.Open);

                    // Don't go any further if the file is too big to work with:
                    if (fs.Length < Convert.ToInt64(Int32.MaxValue))
                    {
                        // Read all of its contents into a byte array:
                        byte[] rawData = new byte[(int)fs.Length];
                        int totalBytes = fs.Read(rawData, 0, (int)fs.Length);
                        fs.Close();

                        // As long as there's something to work with:
                        if (totalBytes > 0)
                        {
                            // Set up the decryption transform and streams:
                            if (rijndael.ValidKeySize(256)) rijndael.KeySize = 256;
                            rijndael.Padding = PaddingMode.PKCS7;
                            ICryptoTransform decryptor =
                                rijndael.CreateDecryptor(HashPassphrase(passphrase), iv);
                            MemoryStream ms = new MemoryStream(rawData);
                            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

                            // Create a new byte array to hold the decrypted data, then read the
                            // decrypted data into it.  The Close()s should remove the padding from
                            // the data automatically.
                            byte[] fromEncrypt = new byte[totalBytes];
                            cs.Read(fromEncrypt, 0, fromEncrypt.Length);
                            cs.Close();
                            ms.Close();

                            // Now we need to decompress the data.  We'll reuse the same memory stream
                            // and wrap a GZipStream around it:
                            ms = new MemoryStream(fromEncrypt);
                            GZipStream unzipper = new GZipStream(ms, CompressionMode.Decompress);
                            // We have no idea how big the uncompressed data will be, but we'll pad
                            // that with some extra space, just in case:
                            byte[] uncompressedData = new byte[fromEncrypt.Length *
                                uncompressArrayPadding];
                            // Now read the bytes from the decompression stream:
                            totalBytes = ReadAllBytesFromStream(unzipper, uncompressedData);
                            unzipper.Close();
                            ms.Close();

                            // Next we need to validate the header to make sure the password was entered
                            // correctly.  Find out how long the header should be in bytes and make sure
                            // the uncompressed, unencrypted data is at least that long:
                            int headerByteLength = utf8.GetBytes(header).Length;
                            if (totalBytes > headerByteLength)
                            {
                                // Read the first so many bytes from the data, as many as should be in
                                // the header, and convert that to a string.  Then compare the string to the
                                // header and make sure they match.  Also make sure that the length of the
                                // data minus the header is something worth processing.
                                string headerCheck = utf8.GetString(uncompressedData, 0,
                                    headerByteLength);
                                if (headerCheck == header &&
                                    uncompressedData.Length - headerByteLength > 0)
                                {
                                    // If everything checks out, then the file was read successfully.
                                    // Copy the bytes minus the header to another array and return it:
                                    byte[] result = new byte[totalBytes - headerByteLength];
                                    Array.Copy(uncompressedData, headerByteLength, result, 0,
                                        result.Length);
                                    return result;
                                }
                                // Header did not match:
                                else throw new SecureFileException("Invalid passphrase for file " +
                                    filename);
                            }
                            // The file didn't have enough bytes to contain the header
                            else throw new SecureFileException("The file " + filename + " appears " +
                                "to be invalid (the file is shorter than the expected header)");
                        }
                        // The file didn't have any data:
                        else throw new SecureFileException("The file " + filename + " appears to " +
                            "be empty");
                    }
                    // The file was over 2GB:
                    else throw new SecureFileException("The file " + filename + " is too large for " +
                        "me to process");
                }
                // If the file didn't exist, just return null:
                else throw new SecureFileException("The file " + filename + " does not exist");

            }
            #region Catch Exceptions
            catch (UnauthorizedAccessException)
            {
                throw new SecureFileException("You do not appear to have the necessar permissions " +
                    "to read the file " + filename);
            }
            catch (System.Security.SecurityException)
            {
                throw new SecureFileException("You do not appear to have the necessar permissions " +
                    "to read the file " + filename);
            }
            catch (ArgumentException)
            {
                throw new SecureFileException("The path to the specified file is either empty or " +
                    "contains invalid characters");
            }
            catch (IOException)
            {
                throw new SecureFileException("An I/O error occurred while trying to read the file " +
                    filename);
            }
            // Rethrow any SecureFileExceptions thrown above:
            catch (SecureFileException sfe) { throw sfe; }
            // Catch anything else that wasn't caught:
            catch (Exception ex)
            {
                throw new SecureFileException(ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// Read all bytes from an encrypted SecureFile
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <param name="iv">A Base64-encoded string containing the initialization vector (IV) used
        /// to encrypt the orginal file</param>
        /// <returns>An array of bytes representing the unecrypted file's contents</returns>
        public static byte[] ReadAllBytes(string filename, string passphrase, string iv)
        {
            try
            {
                return ReadAllBytes(filename, passphrase, Convert.FromBase64String(iv));
            }
            catch (SecureFileException sfe) { throw sfe; }
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The initialization vector string is empty");
            }
            catch (FormatException)
            {
                throw new SecureFileException("The initialization vector string is invalid");
            }
        }

        /// <summary>
        /// Read all bytes from an encrypted SecureFile
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <returns>An array of bytes representing the unecrypted file's contents</returns>
        public static byte[] ReadAllBytes(string filename, string passphrase)
        {
            try
            {
                return ReadAllBytes(filename, passphrase, GenerateIV(passphrase, rijndael.BlockSize));
            }
            catch (SecureFileException sfe) { throw sfe; }
        }

        /// <summary>
        /// Read all data from an encrypted SecureFile as a string encoded with the specified
        /// text encoding
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="encoder">A text encoding scheme, such as ASCII or UTF-8. This must match
        /// the same encoding scheme used when the original strings were encrypted.</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <param name="iv">A byte array containing the initialization vector (IV) used to encrypt
        /// the orginal file</param>
        /// <returns>A string representing the unecrypted file's contents, encoded with the text
        /// encoding specified</returns>
        public static string ReadAsString(string filename, Encoding encoder, string passphrase,
            byte[] iv)
        {
            try
            {
                // Make sure an encoder was specified:
                if (encoder == null) throw new SecureFileException("No string encoding was specified");
                // Read the data from the file as bytes, then convert it to a string if there's
                // actual data to work with.  Otherwise, return null.
                byte[] unencrypted = ReadAllBytes(filename, passphrase, iv);
                if (unencrypted != null && unencrypted.Length > 0)
                    return encoder.GetString(unencrypted);
                else return null;
            }
            catch (SecureFileException sfe) { throw sfe; }
            catch (Exception ex)
            {
                throw new SecureFileException(ex.Message);
            }
        }

        /// <summary>
        /// Read all data from an encrypted SecureFile as a string encoded with the specified
        /// text encoding
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="encoder">A text encoding scheme, such as ASCII or UTF-8. This must match
        /// the same encoding scheme used when the original strings were encrypted.</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <param name="iv">A Base64-encoded string containing the initialization vector (IV) used
        /// to encrypt the orginal file</param>
        /// <returns>A string representing the unecrypted file's contents, encoded with the text
        /// encoding specified</returns>
        public static string ReadAsString(string filename, Encoding encoder, string passphrase,
            string iv)
        {
            try
            {
                return ReadAsString(filename, encoder, passphrase, Convert.FromBase64String(iv));
            }
            catch (SecureFileException sfe) { throw sfe; }
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The initialization vector string is empty");
            }
            catch (FormatException)
            {
                throw new SecureFileException("The initialization vector string is invalid");
            }
        }

        /// <summary>
        /// Read all data from an encrypted SecureFile as a string encoded with the specified
        /// text encoding
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="encoder">A text encoding scheme, such as ASCII or UTF-8. This must match
        /// the same encoding scheme used when the original strings were encrypted.</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <returns>A string representing the unecrypted file's contents, encoded with the text
        /// encoding specified</returns>
        public static string ReadAsString(string filename, Encoding encoder, string passphrase)
        {
            try
            {
                return ReadAsString(filename, encoder, passphrase,
                    GenerateIV(passphrase, rijndael.BlockSize));
            }
            catch (SecureFileException sfe) { throw sfe; }
        }

        /// <summary>
        /// Read all data from an encrypted SecureFile as a UTF-8 encoded string
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <param name="iv">A byte array containing the initialization vector (IV) used to encrypt
        /// the orginal file</param>
        /// <returns>A UTF-8 encoded string representing the unecrypted file's contents</returns>
        public static string ReadAsString(string filename, string passphrase, byte[] iv)
        {
            try
            {
                return ReadAsString(filename, utf8, passphrase, iv);
            }
            catch (SecureFileException sfe) { throw sfe; }
            catch (Exception ex)
            {
                throw new SecureFileException(ex.Message);
            }
        }

        /// <summary>
        /// Read all data from an encrypted SecureFile as a UTF-8 encoded string
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <param name="iv">A Base64-encoded string containing the initialization vector (IV) used
        /// to encrypt the orginal file</param>
        /// <returns>A UTF-8 encoded string representing the unecrypted file's contents</returns>
        public static string ReadAsString(string filename, string passphrase, string iv)
        {
            try
            {
                return ReadAsString(filename, passphrase, Convert.FromBase64String(iv));
            }
            catch (SecureFileException sfe) { throw sfe; }
            catch (ArgumentNullException)
            {
                throw new SecureFileException("The initialization vector string is empty");
            }
            catch (FormatException)
            {
                throw new SecureFileException("The initialization vector string is invalid");
            }
        }

        /// <summary>
        /// Read all data from an encrypted SecureFile as a UTF-8 encoded string
        /// </summary>
        /// <param name="filename">A string containing the path to the file to read</param>
        /// <param name="passphrase">A string containing the passphrase used to encrypt the orginal
        /// file</param>
        /// <returns>A UTF-8 encoded string representing the unecrypted file's contents</returns>
        public static string ReadAsString(string filename, string passphrase)
        {
            try
            {
                return ReadAsString(filename, passphrase, GenerateIV(passphrase, rijndael.BlockSize));
            }
            catch (SecureFileException sfe) { throw sfe; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Hash the given passphrase to create a unique 256-bit key we can use in our encryption
        /// and decryption methods.  Note that the passphrase will be manipulated en route, so
        /// the hash should not be easily reverse engineered.
        /// </summary>
        /// <param name="passphrase">The passphrase string to hash</param>
        /// <returns>A 256-bit byte array suitable for using as a Rijandael key</returns>
        private static byte[] HashPassphrase(string passphrase)
        {
            try
            {
                // Complain if the passphrase is empty:
                if (passphrase == null || passphrase == String.Empty || passphrase == "")
                    throw new SecureFileException("The passphrase is empty");
                // Get the SHA-256 engine:
                SHA256Managed sha = new SHA256Managed();
                // First, for funsies, let's reverse the passphrase.  We'll have to do this with a
                // StringBuilder, as strings are immutable in .NET.  So split the passphrase into
                // a char array and iterate through it backwards, appending each character to the
                // builder.
                StringBuilder sb = new StringBuilder(passphrase.Length);
                char[] ppBits = passphrase.ToCharArray();
                for (int i = passphrase.Length - 1; i >= 0; i--)
                {
                    sb.Append(ppBits[i]);
                }
                // Now let's compute the hash.  What actually gets fed to the has is (a) the reversed
                // passphrase, followed by (b) the salt and (c) the original passphrase again.  We'll
                // use the user-entered value first to give the hash a unique starting point, then
                // throw in the salt which should add an unknown element, and then add some more
                // user data by adding the passphrase again.  Once we have this string, convert that
                // all to UTF-8 bytes and feed that to the hashing engine.  What we should end up with
                // is a nice chunk of semi-random bytes that should be a strong encryption key, but
                // which is repeatable enough that the passphrase can be entered and we'll get the
                // same result.
                return sha.ComputeHash(utf8.GetBytes(Convert.ToBase64String(sha.ComputeHash(utf8.GetBytes(sb.ToString() + salt + passphrase)))));
            }
            catch (SecureFileException sfe) { throw sfe; }
            catch (InvalidOperationException)
            {
                throw new SecureFileException("The Federal Information Processing Standards (FIPS) " +
                    "security setting is enabled on this computer. The secure hashing algorithms " +
                    "required to write encrypted files are unavailable.");
            }
            catch (Exception ex)
            {
                throw new SecureFileException("An unspecified error occurred: " + ex.ToString());
            }
        }

        /// <summary>
        /// Generate the initialization vector (IV) to feed to the encryption engine.
        /// </summary>
        /// <param name="passphrase">A string containing the passphrase</param>
        /// <param name="size">The size in bits of the IV to generate</param>
        /// <returns>An array of bytes containing the generated IV</returns>
        private static byte[] GenerateIV(string passphrase, int size)
        {
            try
            {
                // Complain if the passphrase is empty:
                if (passphrase == null || passphrase == String.Empty || passphrase == "")
                    throw new SecureFileException("The passphrase is empty");
                if (size <= 0 || size % 8 != 0)
                    throw new SecureFileException("The block size of the generated initialization " +
                        "vector (IV) is invalid");
                // To generate the IV, we're going to do some funky things to the passphrase (and the
                // salt) to try and get something close to random but still derived.  To do that, we'll
                // use the RIPEMD-160 hash engine.
                RIPEMD160Managed rip = new RIPEMD160Managed();
                // Hash the password with RIPEMD-160 and convert the bytes to Base64:
                string passripper = Convert.ToBase64String(rip.ComputeHash(utf8.GetBytes(passphrase)));
                // Do the same thing with the salt:
                string saltripper = Convert.ToBase64String(rip.ComputeHash(utf8.GetBytes(salt)));
                // Reverse the passphrase using a StringBuilder, then hash that string as well using
                // the RIPEMD-160 / Base64 combo:
                StringBuilder sb = new StringBuilder(passphrase.Length);
                char[] ppBits = passphrase.ToCharArray();
                for (int i = passphrase.Length - 1; i >= 0; i--)
                {
                    sb.Append(ppBits[i]);
                }
                string passriprev = Convert.ToBase64String(rip.ComputeHash(utf8.GetBytes(sb.ToString())));
                // Now we'll use SHA-512 and has the combined strings we generated above.  We'll end up
                // with a 512-bit IV, which is probably overkill, but it should be good and "randomized"
                // but still derived from the passphrase.
                SHA512Managed sha = new SHA512Managed();
                byte[] hashed =
                    sha.ComputeHash(sha.ComputeHash(utf8.GetBytes(passriprev + saltripper +
                    passripper)));
                // Now just get the first so many bytes (size divided by 8) and return them:
                byte[] iv = new byte[size / 8];
                Array.Copy(hashed, iv, size / 8);
                return iv;
            }
            catch (SecureFileException sfe) { throw sfe; }
            catch (Exception ex)
            {
                throw new SecureFileException("An unspecified error occurred: " + ex.ToString());
            }
        }

        /// <summary>
        /// Read all bytes from the specified stream into the specified byte buffer
        /// </summary>
        /// <param name="s">The stream to read</param>
        /// <param name="buffer">The buffer to read the data into</param>
        /// <returns>How many bytes were actually read</returns>
        private static int ReadAllBytesFromStream(Stream s, byte[] buffer)
        {
            // This is actually taken as an example from the .NET help files, but the example
            // code included an extraneous variable.  An offset was used, but it held the
            // exact same value as totalCount, so I cut that out and just reused totalCount
            // for both instances.
            int totalCount = 0;
            while (true)
            {
                int bytesRead = s.Read(buffer, totalCount, 100);
                if (bytesRead == 0) break;
                totalCount += bytesRead;
            }
            return totalCount;
        }

        #endregion
    }
}
