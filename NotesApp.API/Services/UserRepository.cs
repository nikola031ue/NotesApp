using Microsoft.EntityFrameworkCore;
using NotesApp.API.Contexts;
using NotesApp.API.Entities;
using NotesApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _context.Notes.AddAsync(note);
        }

        public void DeleteNote(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            _context.Notes.Remove(note);
        }

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

        public async Task<Note> GetNoteAsync(Guid noteId)
        {
            if (noteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(noteId));
            }

            return await _context.Notes.Where(a => a.NoteId == noteId).FirstOrDefaultAsync();
        }

        public async Task<Note> GetNoteAsync(Guid noteId, Guid userId)
        {
            if (noteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(noteId));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.Notes.Where(a => a.UserId == userId && a.NoteId == noteId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
                .OrderBy(n => n.Email).ToListAsync();
        }

        public async Task<IEnumerable<Note>> GetNotesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderBy(n => n.Title).ToListAsync();
        }

        public async Task<User> GetUserAsync(string Email, string Password)
        {
            if (Email == null)
            {
                throw new ArgumentNullException(nameof(Email));
            }
            if (Password == null)
            {
                throw new ArgumentNullException(nameof(Password));
            }

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

        }

        public User GetUser(UserResourceParametersDto userResource)
        {
            if (userResource == null)
            {
                throw new ArgumentNullException(nameof(userResource));
            }

            if (string.IsNullOrWhiteSpace(userResource.Email)
                && string.IsNullOrWhiteSpace(userResource.Password))
            {
                throw new ArgumentNullException();
            }

            var collection = _context.Users as IEnumerable<User>;


            var mail = userResource.Email.Trim();
            var password = userResource.Password.Trim();

            collection = collection.Where(a =>
                a.Email.Equals(mail, StringComparison.OrdinalIgnoreCase)
                /*&& a.Password.Equals(password, StringComparison.Ordinal)*/);

            return collection.FirstOrDefault();

        }

        public async Task AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.UserId = new Guid();

            await _context.Users.AddAsync(user);
        }

        public void AddVerification(Verification verification, Guid userId)
        {
            if (verification == null)
            {
                throw new ArgumentNullException(nameof(verification));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            verification.UserId = userId;
            verification.VerificationId = new Guid();

            _context.Verifications.Add(verification);
        }

        public async Task<Verification> GetVerificationAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.Verifications.Where(a => a.UserId == userId).FirstOrDefaultAsync();
        }

        public void UpdateNote(Note note)
        {
        }

        public void UpdateUser(User user)
        {
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.Users.AnyAsync(a => a.UserId == userId);
        }

        public bool UserExists(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _context.Users.Any(a => a.UserId == userId);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
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
