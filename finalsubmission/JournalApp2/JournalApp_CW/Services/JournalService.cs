using Microsoft.EntityFrameworkCore;
using JournalApp_CW.Data;
using JournalApp_CW.Models;

namespace JournalApp_CW.Services
{
    public class JournalService
    {
        private readonly AuthService _auth;

        public JournalService(AuthService auth)
        {
            _auth = auth;
        }

        public async Task InitializeAsync()
        {
            using var context = new JournalDbContext();
            
            await context.Database.EnsureCreatedAsync();
        }

        public async Task<JournalEntry> GetEntryByDateAsync(DateTime date)
        {
            if (!_auth.IsLoggedIn) return null;
            using var context = new JournalDbContext();
            return await context.Entries
                .FirstOrDefaultAsync(e => e.Date.Date == date.Date && e.UserId == _auth.CurrentUser.Id);
        }

        public async Task<List<JournalEntry>> GetEntriesAsync()
        {
            if (!_auth.IsLoggedIn) return new List<JournalEntry>();
            using var context = new JournalDbContext();
            return await context.Entries
                .Where(e => e.UserId == _auth.CurrentUser.Id)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task SaveEntryAsync(JournalEntry entry)
        {
            if (!_auth.IsLoggedIn) return;
            using var context = new JournalDbContext();
            entry.UserId = _auth.CurrentUser.Id;

            var existing = await context.Entries
                .FirstOrDefaultAsync(e => e.Date.Date == entry.Date.Date && e.UserId == entry.UserId);

            if (existing != null)
            {
                existing.Content = entry.Content;
                existing.PrimaryMood = entry.PrimaryMood;
                existing.Tags = entry.Tags;
                context.Entries.Update(existing);
            }
            else
            {
                await context.Entries.AddAsync(entry);
            }
            await context.SaveChangesAsync();
        }
        
        public async Task DeleteAllUserDataAsync()
        {
            if (!_auth.IsLoggedIn) return;
            using var context = new JournalDbContext();
            var entries = await context.Entries.Where(e => e.UserId == _auth.CurrentUser.Id).ToListAsync();
            context.Entries.RemoveRange(entries);
            await context.SaveChangesAsync();
        }

        public async Task DeleteEntryAsync(int entryId)
        {
            if (!_auth.IsLoggedIn) return;
            using var context = new JournalDbContext();
            var entry = await context.Entries.FindAsync(entryId);

            // Ensure the entry belongs to the current user before deleting
            if (entry != null && entry.UserId == _auth.CurrentUser.Id)
            {
                context.Entries.Remove(entry);
                await context.SaveChangesAsync();
            }
        }
    }
}