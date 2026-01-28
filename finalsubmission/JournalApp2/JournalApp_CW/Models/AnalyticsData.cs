using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalApp_CW.Models
{
    public class AnalyticsData
    {
        public int TotalEntries { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int ThisMonthCount { get; set; }
        public double AverageMood { get; set; }
        public string MostFrequentMood { get; set; } = "N/A";
        public int MissedDaysCount { get; set; }
        public double AvgWordCount { get; set; }

        // Charts Data
        public Dictionary<string, double> MoodDistribution { get; set; } = new(); // Mood -> Percentage
        public Dictionary<string, int> TagCounts { get; set; } = new();
        public List<WordCountTrendPoint> WordCountTrend { get; set; } = new();
        public double[] WeeklyTrend { get; set; } = new double[7];
    }

    public class WordCountTrendPoint { public DateTime Date { get; set; } public int Count { get; set; } }
}
