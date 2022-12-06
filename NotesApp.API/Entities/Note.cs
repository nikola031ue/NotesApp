using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp.API.Entities
{

    //public enum ColorEnum
    //{
    //    Red,
    //    Yellow, 
    //    Blue
    //}

    [Table("Notes")]
    public class Note
    {
        [Key]
        public Guid NoteId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        //moze da bude prazna beleska samo sa naslovom
        [MaxLength(2000)]
        public string Content { get; set; }

        [Required]
        [MaxLength(10)]
        public string Color { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
