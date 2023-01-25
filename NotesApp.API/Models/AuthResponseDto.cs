namespace NotesApp.API.Models
{
    public class AuthResponseDto
    {
        public bool IsAuthSuccessful { get; set; }
        public bool IsVerified { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
