using System;
using System.Net.Mail;

namespace NotesApp.API.Helpers
{
    public class MailValidator
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress mail = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
