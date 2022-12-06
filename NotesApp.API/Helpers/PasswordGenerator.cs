using System.Security.Cryptography;
using System;

namespace NotesApp.API.Helpers
{
    public static class PasswordGenerator
    {
        const int lengthPassword = 10;

        public static string GetRandomPassword()
        {
            byte[] rgb = new byte[lengthPassword];
            RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider();
            rngCrypt.GetBytes(rgb);
            return Convert.ToBase64String(rgb);
        }
    }
}
