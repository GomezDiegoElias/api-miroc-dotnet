using System.Security.Cryptography;
using System.Text;

namespace org.apimiroc.core.shared.Utils
{
    public static class PasswordUtils
    {

        // Configuraciones para el hashing
        private const int SaltLength = 6; // Longitud del salt
        private const int HashIterations = 10000; // Número de iteraciones para PBKDF2

        // Genera un salt aleatorio
        public static string GenerateRandomSalt()
        {

            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] salt = new char[SaltLength];

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[SaltLength];
                rng.GetBytes(randomBytes);
                for (int i = 0; i < SaltLength; i++)
                {
                    salt[i] = validChars[randomBytes[i] % validChars.Length];
                }
            }

            return new string(salt);

        }

        // Hashea la contraseña con el salt proporcionado
        public static string HashPasswordWithSalt(string password, string salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password: password + salt,
                salt: Encoding.UTF8.GetBytes(salt),
                iterations: HashIterations,
                hashAlgorithm: HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32); // 32 bytes = 256 bits
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        // Verifica si la contraseña proporcionada coincide con el hash almacenado
        public static bool VerifyPassword(string password, string storedHash, string salt)
        {
            string hashedPassword = HashPasswordWithSalt(password, salt);
            return hashedPassword == storedHash;
        }

    }
}
