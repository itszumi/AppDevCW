using Microsoft.EntityFrameworkCore;
using JournalApp_CW.Data;
using JournalApp_CW.Models;

namespace JournalApp_CW.Services
{
    public class JournalService
    {
        private readonly AuthService _auth;

        // Inject AuthService to know WHO is logged in
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
            // FILTER: Only get entries for THIS user
            return await context.Entries.FirstOrDefaultAsync(e => e.Date.Date == date.Date && e.UserId == _auth.CurrentUser.Id);
        }

        public async Task<List<JournalEntry>> GetEntriesAsync()
        {
            if (!_auth.IsLoggedIn) return new List<JournalEntry>();

            using var context = new JournalDbContext();
            // FILTER: Only get entries for THIS user
            return await context.Entries
                .Where(e => e.UserId == _auth.CurrentUser.Id)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task SaveEntryAsync(JournalEntry entry)
        {
            if (!_auth.IsLoggedIn) return;

            using var context = new JournalDbContext();

            // Assign the current user ID to the entry
            entry.UserId = _auth.CurrentUser.Id;

            if (entry.Id == 0)
            {
                var exists = await context.Entries.AnyAsync(e => e.Date.Date == entry.Date.Date && e.UserId == entry.UserId);
                if (!exists) await context.Entries.AddAsync(entry);
            }
            else
            {
                context.Entries.Update(entry);
            }
            await context.SaveChangesAsync();
        }

        public async Task<(int Total, int Streak, Dictionary<string, int> Moods)> GetStatsAsync()
        {
            if (!_auth.IsLoggedIn) return (0, 0, new Dictionary<string, int>());

            using var context = new JournalDbContext();
            var entries = await context.Entries
                .Where(e => e.UserId == _auth.CurrentUser.Id) // Filter!
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            int total = entries.Count;
            int streak = 0;
            var checkDate = DateTime.Today;

            if (!entries.Any(e => e.Date.Date == checkDate)) checkDate = checkDate.AddDays(-1);

            foreach (var e in entries)
            {
                if (e.Date.Date == checkDate) { streak++; checkDate = checkDate.AddDays(-1); }
                else if (e.Date.Date > checkDate) continue;
                else break;
            }

            var moods = entries
                .Where(e => !string.IsNullOrEmpty(e.PrimaryMood))
                .GroupBy(e => e.PrimaryMood)
                .ToDictionary(g => g.Key, g => g.Count());

            return (total, streak, moods);
        }
    }
}