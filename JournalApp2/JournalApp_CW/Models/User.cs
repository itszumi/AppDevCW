using System.ComponentModel.DataAnnotations;

namespace JournalApp_CW.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // In real world, hash this. For CW, plain text is okay if explained.
    }
}