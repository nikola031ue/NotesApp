using System.ComponentModel.DataAnnotations;

namespace NotesApp.API.Models
{
    public class NoteForCreationDto
    {
        [Required(ErrorMessage = "Fill out a title.")]
        [MaxLength(150, ErrorMessage = " The title shouldn't have motre than 150 characters.")]
        public string Title { get; set; } = "Title";
        
        [Required(ErrorMessage = "Fill out a content.")]
        [MaxLength(2000, ErrorMessage = " The content shouldn't have motre than 2000 characters.")]
        public string Content { get; set; } = "Content";

        [Required(ErrorMessage = "Fill out a color.")]
        [MaxLength(10, ErrorMessage = " The color shouldn't have motre than 10 characters.")]
        public string Color { get; set; } = "#00FFFF";
    }
}
