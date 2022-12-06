using NotesApp.API.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace NotesApp.API.Models
{
    public class UserForCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<NoteDto> Notes { get; set; } = new List<NoteDto>();
    }
}
