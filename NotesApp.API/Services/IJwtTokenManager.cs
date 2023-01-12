namespace NotesApp.API.Services
{
    public interface IJwtTokenManager
    {
        string Authenticate(string email/*, string password*/);
    }
}
