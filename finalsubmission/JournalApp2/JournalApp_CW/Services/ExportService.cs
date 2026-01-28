using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using JournalApp_CW.Data;
using Microsoft.EntityFrameworkCore;
// Create an alias to avoid conflict with MAUI Graphics
using QuestPDFColors = QuestPDF.Helpers.Colors;

namespace JournalApp_CW.Services
{
    public class ExportService
    {
        private readonly AuthService _auth;

        public ExportService(AuthService auth)
        {
            _auth = auth;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateJournalPdf()
        {
            if (!_auth.IsLoggedIn) return null;

            using var context = new JournalDbContext();
            var entries = await context.Entries
                .Where(e => e.UserId == _auth.CurrentUser.Id)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    // Use the alias QuestPDFColors here
                    page.Header().Text($"My Journal: {_auth.CurrentUser.Username}")
                        .FontSize(24).SemiBold().FontColor(QuestPDFColors.Purple.Medium);

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        foreach (var entry in entries)
                        {
                            col.Item().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).PaddingVertical(10).Column(entryCol =>
                            {
                                entryCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(entry.Date.ToString("MMMM dd, yyyy")).SemiBold();
                                    row.RelativeItem().AlignRight().Text(entry.PrimaryMood).Italic();
                                });

                                if (!string.IsNullOrEmpty(entry.Tags))
                                    entryCol.Item().Text($"Tags: {entry.Tags}").FontSize(10).FontColor(QuestPDFColors.Grey.Medium);

                                entryCol.Item().PaddingTop(5).Text(entry.Content);
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}