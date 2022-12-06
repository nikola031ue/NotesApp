using NotesApp.API.Entities;
using NotesApp.API.ResourceParameters;
using System;
using System.Collections.Generic;

namespace NotesApp.API.Services
{
    public interface IUserRepository
    {
        IEnumerable<Note> GetNotes(Guid userId);
        Note GetNote(Guid noteId);
        Note GetNote(Guid noteId, Guid userId);
        void AddNote(Note note, Guid userId);
        void DeleteNote(Note note);
        void UpdateNote(Note note);
        User GetUser(string Email, string Password);
        User GetUser(UserResourceParameters userResource);
        User GetUser(Guid userId); 
        void AddUser(User user);
        void DeleteUser(User user, bool isLogged);
        void UpdateUser(User user);
        bool UserExists(Guid userId);
        bool Save();
    }
}
