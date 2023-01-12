using NotesApp.API.Entities;
using NotesApp.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotesApp.API.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<Note>> GetNotesAsync(Guid userId);
        Task<Note> GetNoteAsync(Guid noteId);
        Task<Note> GetNoteAsync(Guid noteId, Guid userId);
        Task<IEnumerable<User>> GetUsersAsync();
        void AddNote(Note note, Guid userId);
        void DeleteNote(Note note);
        void UpdateNote(Note note);
        Task<User> GetUserAsync(string Email, string Password);
        User GetUser(UserResourceParametersDto userResource);
        Task<User> GetUserAsync(Guid userId); 
        Task AddUser(User user);
        void AddVerification(Verification verification, Guid userId);
        Task<Verification> GetVerificationAsync(Guid userId);
        void DeleteUser(User user, bool isLogged);
        void UpdateUser(User user);
        Task<bool> UserExistsAsync(Guid userId);
        bool UserExists(Guid userId);
        Task SaveAsync();
    }
}
