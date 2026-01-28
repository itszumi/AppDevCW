using System.ComponentModel.DataAnnotations;

namespace JournalApp_CW.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }
        public string Content { get; set; } = string.Empty;
        public string PrimaryMood { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}