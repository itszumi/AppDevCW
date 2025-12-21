using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Today;
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

}
