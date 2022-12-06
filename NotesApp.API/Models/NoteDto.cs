using System;

namespace NotesApp.API.Models
{
    public class NoteDto
    {
        public Guid NoteId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Color { get; set; }
        public Guid UserId { get; set; }
    }
}
