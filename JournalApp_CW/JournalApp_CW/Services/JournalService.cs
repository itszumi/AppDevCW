using Microsoft.EntityFrameworkCore;
using JournalApp_CW.Data;
using JournalApp_CW.Models;

namespace JournalApp_CW.Services
{
    public class JournalService
    {
        // 1. Initialize the Database
        public async Task InitializeAsync()
        {
            using var context = new JournalDbContext();
            await context.Database.EnsureCreatedAsync();
        }

        // 2. Get Entry by Date
        public async Task<JournalEntry> GetEntryByDateAsync(DateTime date)
        {
            using var context = new JournalDbContext();
            return await context.Entries
                .FirstOrDefaultAsync(e => e.Date.Date == date.Date);
        }

        // 3. Save (Create or Update)
        public async Task SaveEntryAsync(JournalEntry entry)
        {
            using var context = new JournalDbContext();

            if (entry.Id == 0)
            {
                // Verify no duplicate exists
                var exists = await context.Entries.AnyAsync(e => e.Date.Date == entry.Date.Date);
                if (!exists)
                {
                    await context.Entries.AddAsync(entry);
                }
            }
            else
            {
                context.Entries.Update(entry);
            }

            await context.SaveChangesAsync();
        }

        // 4. Delete
        public async Task DeleteEntryAsync(int id)
        {
            using var context = new JournalDbContext();
            var entry = await context.Entries.FindAsync(id);
            if (entry != null)
            {
                context.Entries.Remove(entry);
                await context.SaveChangesAsync();
            }
        }
    }
}