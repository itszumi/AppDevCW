using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CW.Models;
using Microsoft.EntityFrameworkCore.Sqlite;
namespace CW.Data
{
    public partial class JournalDbContext:DbContext
    {
        public DbSet<JournalEntry> Entries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

    }
}
