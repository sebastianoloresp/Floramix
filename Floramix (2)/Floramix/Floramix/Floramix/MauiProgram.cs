using FloraMix.Services;
using FloraMix.Shared.Services;
using Microsoft.Extensions.Logging;

namespace FloraMix
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("PlayfairDisplay-Regular.ttf", "PlayfairRegular");
                    fonts.AddFont("PlayfairDisplay-Italic.ttf", "PlayfairItalic");
                });

            // Add device-specific services used by the FloraMix.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IShopService, MauiShopService>();
            builder.Services.AddSingleton<ShopStateService>();

            // Database service - single shared instance across the whole app
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}