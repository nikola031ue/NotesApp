using NotesApp.API.Contexts;
using NotesApp.API.Entities;
using NotesApp.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotesApp.API.Services
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddNote(Note note, Guid userId)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            note.UserId = userId;
            _context.Notes.Add(note);
        }

        public void DeleteNote(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            _context.Notes.Remove(note);
        }

        //TODO mozda moze bolje
        public void DeleteUser(User user, bool isLogged)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (isLogged)
            {
                _context.Users.Remove(user);
            }
            else
            {
                throw new Exception("Please login first");
            }
        }

        public Note GetNote(Guid noteId)
        {
            if (noteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(noteId));
            }

            return _context.Notes.Where(a => a.NoteId == noteId).FirstOrDefault();
        }

        public Note GetNote(Guid noteId, Guid userId)
        {
            if (noteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(noteId));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _context.Notes.Where(a => a.UserId == userId && a.NoteId == noteId).FirstOrDefault();
        }

        public IEnumerable<Note> GetNotes(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _context.Notes
                .Where(n => n.UserId == userId)
                .OrderBy(n => n.Title).ToList();
        }

        public User GetUser(string Email, string Password)
        {
            if (Email == null)
            {
                throw new ArgumentNullException(nameof(Email));
            }
            if (Password == null)
            {
                throw new ArgumentNullException(nameof(Password));
            }

            return _context.Users
                .FirstOrDefault(u => u.Email == Email && u.Password == Password);
        }

        public User GetUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _context.Users
                .FirstOrDefault(u => u.UserId == userId);

        }

        public User GetUser(UserResourceParameters userResource)
        {
            if (userResource == null)
            {
                throw new ArgumentNullException(nameof(userResource));
            }

            if (string.IsNullOrWhiteSpace(userResource.Email)
                && string.IsNullOrWhiteSpace(userResource.Password))
            {
                throw new Exception("Please fill the box.");
            }

            var collection = _context.Users as IEnumerable<User>;


            if (!string.IsNullOrWhiteSpace(userResource.Email) && !string.IsNullOrWhiteSpace(userResource.Password))
            {
                var mail = userResource.Email.Trim();
                var password = userResource.Password.Trim();

                collection = collection.Where(a =>
                    a.Email.Equals(mail, StringComparison.OrdinalIgnoreCase)
                    && a.Password.Equals(password, StringComparison.Ordinal)
                );

                return collection != null ? collection.First() : null;
            }
            return null;
        }
        
        public void AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.UserId = new Guid();

            _context.Users.Add(user);
        }

        public void UpdateNote(Note note)
        {
        }

        public void UpdateUser(User user)
        {
        }

        public bool UserExists(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _context.Users.Any(a => a.UserId == userId);
        }
        
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }


    }
}
