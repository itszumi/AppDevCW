using Microsoft.EntityFrameworkCore;
using JournalApp_CW.Data;
using JournalApp_CW.Models;

namespace JournalApp_CW.Services
{
    public class AnalyticsService
    {
        private readonly AuthService _auth;
        public AnalyticsService(AuthService auth) { _auth = auth; }

        public async Task<AnalyticsData> GetAnalyticsAsync(DateTime? start = null, DateTime? end = null)
        {
            if (!_auth.IsLoggedIn) return new AnalyticsData();

            using var context = new JournalDbContext();
            var query = context.Entries.Where(e => e.UserId == _auth.CurrentUser.Id);

            // Apply Date Filter
            if (start.HasValue) query = query.Where(e => e.Date >= start.Value);
            if (end.HasValue) query = query.Where(e => e.Date <= end.Value);

            var entries = await query.OrderBy(e => e.Date).ToListAsync();
            var data = new AnalyticsData { TotalEntries = entries.Count };
            if (!entries.Any()) return data;

            // 1. Mood Calculations
            var moodGroups = entries.GroupBy(e => e.PrimaryMood)
                .OrderByDescending(g => g.Count());
            data.MostFrequentMood = moodGroups.FirstOrDefault()?.Key ?? "None";

            // Mood Distribution (Simplified to Pos/Neu/Neg)
            int pos = entries.Count(e => e.PrimaryMood == "Amazing" || e.PrimaryMood == "Happy");
            int neu = entries.Count(e => e.PrimaryMood == "Neutral");
            int neg = entries.Count(e => e.PrimaryMood == "Tired" || e.PrimaryMood == "Sad");
            data.MoodDistribution.Add("Positive", (double)pos / entries.Count * 100);
            data.MoodDistribution.Add("Neutral", (double)neu / entries.Count * 100);
            data.MoodDistribution.Add("Negative", (double)neg / entries.Count * 100);

            // 2. Streaks (Current & Longest)
            int current = 0, max = 0, temp = 0;
            var allDates = entries.Select(e => e.Date.Date).Distinct().OrderByDescending(d => d).ToList();

            // Current Streak
            var check = DateTime.Today;
            if (!allDates.Contains(check)) check = check.AddDays(-1);
            while (allDates.Contains(check)) { current++; check = check.AddDays(-1); }
            data.CurrentStreak = current;

            // Longest Streak
            var ascDates = allDates.OrderBy(d => d).ToList();
            for (int i = 0; i < ascDates.Count; i++)
            {
                if (i > 0 && ascDates[i] == ascDates[i - 1].AddDays(1)) temp++;
                else temp = 1;
                if (temp > max) max = temp;
            }
            data.LongestStreak = max;

            // 3. Missed Days (Within the filtered range)
            var rangeStart = start ?? entries.First().Date;
            var rangeEnd = end ?? DateTime.Today;
            int totalDaysInRange = (rangeEnd - rangeStart).Days + 1;
            data.MissedDaysCount = totalDaysInRange - allDates.Count(d => d >= rangeStart.Date && d <= rangeEnd.Date);

            // 4. Word Count Trends
            // Assuming entry has a 'Content' string property
            var wordCounts = entries.Select(e => e.Content?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length ?? 0).ToList();
            data.AvgWordCount = Math.Round(wordCounts.Average(), 1);
            data.WordCountTrend = entries.Select(e => new WordCountTrendPoint
            {
                Date = e.Date,
                Count = e.Content?.Split(' ').Length ?? 0
            }).TakeLast(10).ToList();

            // 5. Tags
            var tags = entries.SelectMany(e => (e.Tags ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)).Select(t => t.Trim());
            data.TagCounts = tags.GroupBy(t => t).OrderByDescending(g => g.Count()).Take(5).ToDictionary(g => g.Key, g => g.Count());

            return data;
        }
    }
}