using Microsoft.Extensions.Logging;
using JournalApp_CW.Services; // IMPORTANT
using JournalApp_CW.Data;     // IMPORTANT

namespace JournalApp_CW
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            // --- MAKE SURE THESE 2 LINES ARE HERE ---
            builder.Services.AddSingleton<JournalService>();
            builder.Services.AddSingleton<AuthService>();

            return builder.Build();
        }
    }
}