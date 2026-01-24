using Microsoft.EntityFrameworkCore;
using JournalApp_CW.Models;

namespace JournalApp_CW.Data
{
    public class JournalDbContext : DbContext
    {
        public DbSet<JournalEntry> Entries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Set the path to the local app data folder
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal_ef.db");

            // Configure SQLite
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}