using System.Security.Cryptography;
using System.Text;

namespace capyborrowProject.Helpers
{
    public class PasswordHelper
    {
        private const int SaltSize = 16; // 128-bit
        private const int KeySize = 32;  // 256-bit
        private const int Iterations = 10000;

        /// <summary>
        /// Hashes a password using PBKDF2 with a generated salt.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <returns>A Base64-encoded string containing the salt and hashed password.</returns>
        public static string HashPassword(string password)
        {
            // Generate a salt
            using var rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            // Generate the hash
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Combine the salt and hash
            byte[] hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }


        /// <summary>
        /// Verifies a password against a stored hash.
        /// </summary>
        /// <param name="password">The plaintext password to verify.</param>
        /// <param name="storedHash">The stored hash to compare against.</param>
        /// <returns>True if the password matches; otherwise, false.</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            if (string.IsNullOrWhiteSpace(storedHash))
                throw new ArgumentException("Stored hash cannot be null or empty.", nameof(storedHash));

            byte[] hashBytes = Convert.FromBase64String(storedHash);

            if (hashBytes.Length != SaltSize + KeySize)
                throw new ArgumentException("Invalid hash format.", nameof(storedHash));

            // Extract the salt
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Extract the stored hash
            byte[] storedPasswordHash = new byte[KeySize];
            Array.Copy(hashBytes, SaltSize, storedPasswordHash, 0, KeySize);

            // Hash the incoming password with the same salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] passwordHash = pbkdf2.GetBytes(KeySize);

            // Compare the hashes
            return CryptographicEquals(storedPasswordHash, passwordHash);
        }

        /// <summary>
        /// Securely compares two byte arrays to prevent timing attacks.
        /// </summary>
        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            /// Or there is more sequre approach:
            ///return CryptographicOperations.FixedTimeEquals(a, b);
            ///"CryptographicOperations.FixedTimeEquals" is designed for constant-time comparisons to prevent timing attacks.

            if (a.Length != b.Length)
                return false;

            bool areEqual = true;
            for (int i = 0; i < a.Length; i++)
            {
                areEqual &= a[i] == b[i];
            }
            return areEqual;
        }
    }
}



//namespace capyborrowProject.Helpers
//{
//    public class PasswordHelper
//    {
//        private const int SaltSize = 16; // 128-bit
//        private const int KeySize = 32;  // 256-bit
//        private const int Iterations = 10000;

//        public static string HashPassword(string password)
//        {
//            // Generate a salt
//            using var rng = new RNGCryptoServiceProvider();
//            byte[] salt = new byte[SaltSize];
//            rng.GetBytes(salt);

//            // Generate the hash
//            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
//            byte[] hash = pbkdf2.GetBytes(KeySize);

//            // Combine the salt and hash
//            byte[] hashBytes = new byte[SaltSize + KeySize];
//            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
//            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

//            return Convert.ToBase64String(hashBytes);
//        }
//    }
//}
