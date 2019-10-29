using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class HashPasswordBL
    {
        // The following constants may be changed without breaking existing hashes.
        public const int SALT_BYTES = 24;
        public const int HASH_BYTES = 24;
        public const int PBKDF2_ITERATIONS = 1000;

        public const int SALT_INDEX = 0;
        public const int PBKDF2_INDEX = 1;     

        /// <summary>
        /// Creates a salted PBKDF2 hash of the password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hash of the password.</returns>
        public string GetHashedPassword(string password)
        {
            // Generate a random salt
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_BYTES];
            csprng.GetBytes(salt);

            // Hash the password and encode the parameters
            byte[] hash = PBKDF2(password, salt, HASH_BYTES);
            return Convert.ToBase64String(salt) + ":" +
                    Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Validates user credentials based on userID and password provided.
        /// </summary>
        /// <param name="userID">The userID to check.</param>
        /// <param name="password">The password to check.</param>
        /// <returns>True if the password is correct. False otherwise.</returns>
        public bool ValidateUser(string userID, string password)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                string _alreadyHashedPassword = String.Empty;

                var _userExists = _context.Users.SingleOrDefault(u => u.userID == userID.ToLower() && !(u.isDeleted ?? false) && u.isActive);

                if (_userExists !=null)
                {
                    _alreadyHashedPassword = _userExists.password;

                    if (!String.IsNullOrEmpty(_alreadyHashedPassword))
                    {
                        string _correctHash = _alreadyHashedPassword.ToString();

                        // Extract the parameters from the hash
                        char[] delimiter = { ':' };
                        string[] split = _correctHash.Split(delimiter);

                        byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
                        byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

                        byte[] testHash = PBKDF2(password, salt, hash.Length);
                        return SlowEquals(hash, testHash);
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Compares two byte arrays in length-constant time. This comparison
        /// method is used so that password hashes cannot be extracted from
        /// on-line systems using a timing attack and then attacked off-line.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>True if both byte arrays are equal. False otherwise.</returns>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private static byte[] PBKDF2(string password, byte[] salt, int outputBytes)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = PBKDF2_ITERATIONS;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}
