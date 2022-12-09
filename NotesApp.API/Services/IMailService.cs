using System.Threading.Tasks;

namespace NotesApp.API.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toMail, string subject, string content);
    }
}
