/* HashEngine.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          December 1, 2009
 * PROJECT:       Cryptnos
 * .NET VERSION:  2.0
 * REQUIRES:      See using statements
 * REQUIRED BY:   Form1
 * 
 * The core hashing engine for Cryptnos.  This is in many respects a watered-down version of the
 * HashEngine class in WinHasher (Core), stripped of many of its extra methods that aren't needed
 * for Cryptnos.  It also includes an extra feature not included in WinHasher but important for
 * Cryptnos to function.
 * 
 * HashEngine is a static class, so it does not need to be instantiated.  Instead, call one of
 * the HashString() methods directly.  Each takes an enumerated hash algorithm, an input string,
 * and an iteration count (which must be a postitive integer greater than zero).  One version
 * lets the caller specify the text encoding, while the other assumes the system default.  The
 * input string is converted to raw bytes using the specified encoding and is then fed to the
 * selected hashing algorithm at least once.  If the iteration count is two or greater, the
 * result of the previous hash is passed back through the algorithm again until the iteration
 * count is exhausted.  The final resulting hash is then encoded using Base64 and returned as
 * a string to the caller.  Base64 is used because it provides us with the highest number of
 * possible characters per slot in the result, making for a stronger pseudo-random password.
 * 
 * This program is Copyright 2009, Jeffrey T. Darlington.
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
using System.Text;
using System.Security.Cryptography;

namespace com.gpfcomics.Cryptnos
{
    /// <summary>
    /// An enumeration of the hashes the HashEngine supports.
    /// </summary>
    public enum Hashes
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        RIPEMD160,
        Whirlpool,
        Tiger
    }

    /// <summary>
    /// The hash engine core.  Note that this is a static class, and it should be used like a
    /// utility library.  There's no need to create instances; just call the static methods.
    /// </summary>
    public static class HashEngine
    {
        #region Public Static Methods

        /// <summary>
        /// Computes the cryptographic hash of an input string over a specified number of
        /// iterations and returns a Base64-encoded result
        /// </summary>
        /// <param name="hash">The cryptographic hash algorithm to use, specified by one of
        /// the <see cref="Hashes"/> enumeration values.</param>
        /// <param name="encoding">The text encoding of the input text</param>
        /// <param name="text">The input string to hash.</param>
        /// <param name="count">An iteration count.  The hash will be applied to the string
        /// this many times.  This must be a positive integer.</param>
        /// <returns>A Base64-encoded string representing the hash of the input string</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashString(Hashes hash, Encoding encoding, string text, int count)
        {
            // If the iteration count is less than zero, throw an exception.  We could use
            // an unsigned integer here, but that's overkill since we should never want to
            // perform the hash over two billion times!
            if (count <= 0) throw new HashEngineException("The number of interations must " +
                "be a positive integer.");
            // Asbestos underpants:
            try
            {
                // Get the hash algorithm:
                HashAlgorithm hasher = GetHashAlgorithm(hash);
                // Convert the input string to raw bytes using the default system encoding.
                // We could let the user specify the encoding here, but for now we'll let
                // that slide.
                byte[] theHash = encoding.GetBytes(text);
                // Iterate the hash so many times.  Note this implies the hash will be done
                // at least once.
                for (int i = 0; i < count; i++)
                {
                    // Compute the hash of the current value of the byte array.  For the
                    // first pass, this is the value of the input string; for every subsequent
                    // pass, it's the previous hash value.
                    hasher.TransformFinalBlock(theHash, 0, theHash.Length);
                    theHash = hasher.Hash;
                    hasher.Initialize();
                }
                // Build the output.  Note that for our purposes, we'll always be returning
                // Base64 here, since that gives us a strong pseudo-random password.
                return Convert.ToBase64String(theHash);
            }
            // If anything blows up, rewrap the error in a HashEngineException and send it
            // back to the caller:
            catch (Exception ex) { throw new HashEngineException(ex.Message); }
        }

        /// <summary>
        /// Computes the cryptographic hash of an input string over a specified number of
        /// iterations and returns a Base64-encoded result
        /// </summary>
        /// <param name="hash">The cryptographic hash algorithm to use, specified by one of
        /// the <see cref="Hashes"/> enumeration values.</param>
        /// <param name="text">The input string to hash.  The system default encoding is
        /// assumed with this version of this method.</param>
        /// <param name="count">An iteration count.  The hash will be applied to the string
        /// this many times.  This must be a positive integer.</param>
        /// <returns>A Base64-encoded string representing the hash of the input string</returns>
        /// <exception cref="HashEngineException">Thrown whenever anything bad happens when the
        /// hash is computed</exception>
        public static string HashString(Hashes hash, string text, int count)
        {
            return HashString(hash, Encoding.Default, text, count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Given an item from the Hashes enumeration, get the HashAlgorithm associated with it.
        /// </summary>
        /// <param name="hash">A Hashes enumeration item</param>
        /// <returns>The HashAlgorithm that matches the enumerated hash</returns>
        private static HashAlgorithm GetHashAlgorithm(Hashes hash)
        {
            HashAlgorithm hasher = null;
            switch (hash)
            {
                case Hashes.MD5:
                    hasher = new MD5CryptoServiceProvider();
                    break;
                case Hashes.SHA1:
                    hasher = new SHA1CryptoServiceProvider();
                    break;
                case Hashes.SHA256:
                    hasher = new SHA256Managed();
                    break;
                case Hashes.SHA384:
                    hasher = new SHA384Managed();
                    break;
                case Hashes.SHA512:
                    hasher = new SHA512Managed();
                    break;
                case Hashes.RIPEMD160:
                    hasher = new RIPEMD160Managed();
                    break;
                case Hashes.Whirlpool:
                    hasher = new WhirlpoolManaged();
                    break;
                case Hashes.Tiger:
                    hasher = new TigerManaged();
                    break;
                // If we didn't get something we expected, default to MD5:
                default:
                    hasher = new MD5CryptoServiceProvider();
                    break;
            }
            return hasher;
        }

        #endregion
    }
}
