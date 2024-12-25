using System.Security.Cryptography;
using System.Text;

namespace capyborrowProject.Helpers
{
    public class PasswordHelper
    {
        private const int SaltSize = 16; // 128-bit
        private const int KeySize = 32;  // 256-bit
        private const int Iterations = 10000;

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
    }
}
