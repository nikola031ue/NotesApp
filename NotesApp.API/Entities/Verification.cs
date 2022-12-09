using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp.API.Entities
{
    [Table("Verification")]
    public class Verification
    {
        [Key]
        public Guid VerificationId { get; set; }

        public int VerificationCode { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
