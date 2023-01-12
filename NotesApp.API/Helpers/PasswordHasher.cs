using System;
using System.Security.Cryptography;
using System.Text;

namespace NotesApp.API.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            SHA256 hash = SHA256.Create();
            var passwordBytes = Encoding.Default.GetBytes(password);
            var hashedPassword = hash.ComputeHash(passwordBytes);

            return Convert.ToBase64String(hashedPassword);
        }
    }
}
